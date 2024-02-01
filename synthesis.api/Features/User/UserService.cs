using AutoMapper;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

public interface IUserService
{
    Task<Response<UserDto>> RegisterUser(RegisterUserDto registerRequest);

    Task<Response<UserDto>> GetUserById(Guid id);

    Task<Response<UserDto>> UpdateUser(Guid id, UpdateUserDto updateRequest);

    Task<Response<UserDto>> PatchUser(Guid id, UpdateUserDto patchDtos);

    Task<Response<UserDto>> DeleteUser(Guid id);

}

public class UserService : IUserService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;

    public UserService(RepositoryContext repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;

    }

    public async Task<Response<UserDto>> RegisterUser(RegisterUserDto registerRequest)
    {

        var user = _mapper.Map<UserModel>(registerRequest);

        var validationResult = await new UserValidator(_repository).ValidateAsync(user);

        if (!validationResult.IsValid)
        {
            return new Response<UserDto>(false, "failed to register user", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.Users.AddAsync(user);
        await _repository.SaveChangesAsync();

        var userToReturn = _mapper.Map<UserDto>(user);

        return new Response<UserDto>(true, "user registered successfully", value: userToReturn);

    }

    public async Task<Response<UserDto>> GetUserById(Guid id)
    {
        var user = await _repository.Users.FindAsync(id);

        if (user == null) return new Response<UserDto>(false, "get user failed", errors: [$"user with id:{id} not found"]);

        var userToReturn = _mapper.Map<UserDto>(user);

        return new Response<UserDto>(true, "get user success", value: userToReturn);
    }

    public async Task<Response<UserDto>> UpdateUser(Guid id, UpdateUserDto updateRequest)
    {
        var user = await _repository.Users.FindAsync(id);
        if (user == null) return new Response<UserDto>(false, "update user failed", errors: [$"user with id{id} not found"]);

        var updatedUser = _mapper.Map(updateRequest, user);

        var validationResult = await new UserValidator(_repository, user).ValidateAsync(updatedUser);
        if (!validationResult.IsValid)
        {
            return new Response<UserDto>(false, "update user failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new Response<UserDto>(true, "user update success");


    }

    public async Task<Response<UserDto>> PatchUser(Guid id, UpdateUserDto patchRequest)
    {
        var user = await _repository.Users.FindAsync(id);
        if (user == null) return new Response<UserDto>(false, "delete user failed", errors: [$"user with id{id} not found"]);

        var existingUser = new UserModel() { UserName = user.UserName, Email = user.Email };

        var userToBePatched = _mapper.Map<UpdateUserDto>(user);

        foreach (var prop in patchRequest.GetType().GetProperties())
        {
            var value = prop.GetValue(patchRequest);

            if (value != null)
            {
                prop.SetValue(userToBePatched, value);
            }
        }

        var patchedUser = _mapper.Map(userToBePatched, user);

        var validationResult = await new UserValidator(_repository, existingUser).ValidateAsync(patchedUser);
        if (!validationResult.IsValid)
        {
            return new Response<UserDto>(false, "update user failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }


        await _repository.SaveChangesAsync();

        return new Response<UserDto>(true, "patch user success");

    }

    public async Task<Response<UserDto>> DeleteUser(Guid id)
    {
        var user = await _repository.Users.FindAsync(id);
        if (user == null) return new Response<UserDto>(false, "delete user failed", errors: [$"user with id{id} not found"]);

        _repository.Users.Remove(user);
        _repository.SaveChanges();

        return new Response<UserDto>(true, "delete user success");
    }


}
