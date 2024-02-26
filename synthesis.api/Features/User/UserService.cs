using System.Collections.Immutable;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;
using synthesis.api.Services.BlobStorage;

public interface IUserService
{

    Task<GlobalResponse<UserDto>> GetUserById(Guid id);

    Task<GlobalResponse<UserDto>> UpdateUser(Guid id, UpdateUserDto updateRequest);

    Task<GlobalResponse<UserDto>> PatchUser(Guid id, UpdateUserDto patchRequest);

    Task<GlobalResponse<UserDto>> DeleteUser(Guid id);

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


    public async Task<GlobalResponse<UserDto>> GetUserById(Guid id)
    {
        var user = await _repository.Users
        .Where(u => u.Id == id)
        .Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.UserName,
            AvatarUrl = u.AvatarUrl,
            Email = u.Email,
            MemberProfiles = u.MemberProfiles.Select(x => new MemberDto()
            {
                Id = x.Id,
                Team = new TeamDto()
                {
                    Id = x.Team.Id,
                    Name = x.Team.Name,
                    LogoUrl = x.Team.LogoUrl
                },
                Roles = x.Roles
            }).ToList()

        }).SingleOrDefaultAsync();


        if (user == null) return new GlobalResponse<UserDto>(false, "get user failed", errors: [$"user with id:{id} not found"]);

        return new GlobalResponse<UserDto>(true, "get user success", value: user);
    }

    public async Task<GlobalResponse<UserDto>> UpdateUser(Guid id, UpdateUserDto updateRequest)
    {
        var user = await _repository.Users.FindAsync(id);
        if (user == null) return new GlobalResponse<UserDto>(false, "update user failed", errors: [$"user with id{id} not found"]);

        var updatedUser = _mapper.Map(updateRequest, user);

        var validationResult = await new UserValidator().ValidateAsync(updatedUser);
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

        var validationResult = await new UserValidator().ValidateAsync(patchedUser);
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
