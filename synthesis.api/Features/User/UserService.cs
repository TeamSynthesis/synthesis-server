
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

public interface IUserService
{
    Task<Response<UserDto>> GetUserById(Guid id);

    Task<Response<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto updateRequest);

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

        await _repository.SaveChangesAsync();

        return new Response<UserDto>(true, "user update success");


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
