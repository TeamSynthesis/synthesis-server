
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
}
//ae8bf7fb-7295-4169-bf3a-6f2324c3655c