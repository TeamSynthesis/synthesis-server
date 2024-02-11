using System.Collections.Immutable;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;
using synthesis.api.Services.BlobStorageService;

public interface IUserService
{
    Task<GlobalResponse<UserDto>> RegisterUser(RegisterUserDto registerRequest);

    Task<GlobalResponse<UserProfileDto>> GetUserById(Guid id);

    Task<GlobalResponse<UserDto>> UpdateUser(Guid id, UpdateUserDto updateRequest);

    Task<GlobalResponse<UserDto>> PatchUser(Guid id, UpdateUserDto patchRequest);

    Task<GlobalResponse<UserDto>> DeleteUser(Guid id);

}

public class UserService : IUserService
{
    private readonly RepositoryContext _repository;
    private readonly AzureBlobService _blobService;
    private readonly IMapper _mapper;

    public UserService(RepositoryContext repository, IMapper mapper, AzureBlobService blobService)
    {
        _repository = repository;
        _mapper = mapper;

        _blobService = blobService;

    }

    public async Task<GlobalResponse<UserDto>> RegisterUser(RegisterUserDto registerRequest)
    {

        var user = _mapper.Map<UserModel>(registerRequest);

        var validationResult = await new UserValidator(_repository).ValidateAsync(user);

        if (!validationResult.IsValid)
        {
            return new GlobalResponse<UserDto>(false, "failed to register user", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        if (registerRequest.Avatar == null)
        {
            user.AvatarUrl = $"https://eu.ui-avatars.com/api/?name={user.UserName}&size=250";
        }
        else
        {
            var avatarBlob = await _blobService.Upload(registerRequest.Avatar);

            if (avatarBlob?.Data == null)
            {
                user.AvatarUrl = $"https://eu.ui-avatars.com/api/?name={user.UserName}&size=250";
            }
            else
            {
                user.AvatarUrl = avatarBlob.Data.Uri;
            }

        }



        await _repository.Users.AddAsync(user);
        await _repository.SaveChangesAsync();

        var userToReturn = _mapper.Map<UserDto>(user);

        return new GlobalResponse<UserDto>(true, "user registered successfully", value: userToReturn);

    }

    public async Task<GlobalResponse<UserProfileDto>> GetUserById(Guid id)
    {
        var user = await _repository.Users
        .Where(u => u.Id == id)
        .Include(u => u.MemberProfiles).ThenInclude(m => m.Organisation)
        .Select(u => new UserProfileDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Username = u.UserName,
            AvatarUrl = u.AvatarUrl,
            Email = u.Email,
            MemberProfiles = _mapper.Map<List<MemberProfileDto>>(u.MemberProfiles)

        }).SingleOrDefaultAsync();


        if (user == null) return new GlobalResponse<UserProfileDto>(false, "get user failed", errors: [$"user with id:{id} not found"]);

        return new GlobalResponse<UserProfileDto>(true, "get user success", value: user);
    }

    public async Task<GlobalResponse<UserDto>> UpdateUser(Guid id, UpdateUserDto updateRequest)
    {
        var user = await _repository.Users.FindAsync(id);
        if (user == null) return new GlobalResponse<UserDto>(false, "update user failed", errors: [$"user with id{id} not found"]);

        var updatedUser = _mapper.Map(updateRequest, user);

        var validationResult = await new UserValidator(_repository, user).ValidateAsync(updatedUser);
        if (!validationResult.IsValid)
        {
            return new GlobalResponse<UserDto>(false, "update user failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new GlobalResponse<UserDto>(true, "user update success");

    }

    public async Task<GlobalResponse<UserDto>> PatchUser(Guid id, UpdateUserDto patchRequest)
    {
        var user = await _repository.Users.FindAsync(id);
        if (user == null) return new GlobalResponse<UserDto>(false, "delete user failed", errors: [$"user with id{id} not found"]);

        var existingUser = new UserModel() { UserName = user.UserName, Email = user.Email };

        var userToBePatched = _mapper.Map<UpdateUserDto>(user);

        var patchedUserDto = Patcher.Patch(patchRequest, userToBePatched);

        var patchedUser = _mapper.Map(patchedUserDto, user);

        var validationResult = await new UserValidator(_repository, existingUser).ValidateAsync(patchedUser);
        if (!validationResult.IsValid)
        {
            return new GlobalResponse<UserDto>(false, "update user failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new GlobalResponse<UserDto>(true, "patch user success");
    }

    public async Task<GlobalResponse<UserDto>> DeleteUser(Guid id)
    {
        var user = await _repository.Users.FindAsync(id);
        if (user == null) return new GlobalResponse<UserDto>(false, "delete user failed", errors: [$"user with id{id} not found"]);

        _repository.Users.Remove(user);
        _repository.SaveChanges();

        return new GlobalResponse<UserDto>(true, "delete user success");
    }


}
