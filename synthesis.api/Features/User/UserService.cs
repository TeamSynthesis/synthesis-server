using System.Collections.Immutable;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using Octokit;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;
using synthesis.api.Services.BlobStorage;
using Synthesis.Api.Services.BlobStorage;

public interface IUserService
{

    Task<GlobalResponse<UserDto>> GetUserById(Guid id);

    Task<GlobalResponse<UserDto>> UpdateUser(Guid id, UpdateUserDto updateRequest);

    Task<GlobalResponse<UserDto>> PatchUser(Guid id, UpdateUserDto patchRequest);

    Task<GlobalResponse<UserDto>> PostUserDetails(Guid id, PostUserDetailsDto postCommand);

    Task<GlobalResponse<UserDto>> PostUserSkills(Guid id, List<string> postSkillsCommand);

    Task<GlobalResponse<UserDto>> DeleteUser(Guid id);

}

public class UserService : IUserService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;
    private readonly R2CloudStorage _r2Cloud;

    public UserService(RepositoryContext repository, IMapper mapper, R2CloudStorage r2Cloud)
    {
        _repository = repository;
        _mapper = mapper;
        _r2Cloud = r2Cloud;
    }


    public async Task<GlobalResponse<UserDto>> GetUserById(Guid id)
    {

        var user = await _repository.Users
        .Where(u => u.Id == id)
        .Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            AvatarUrl = u.AvatarUrl,
            Email = u.Email,
            FullName = u.FullName,
            OnBoarding = u.OnBoardingProgress.GetDisplayName(),
            EmailConfirmed = u.EmailConfirmed,
            Profession = u.Profession,
            Skills = u.Skills,
            MemberProfiles = u.MemberProfiles.Select(x => new MemberDto()
            {
                Id = x.Id,
                Team = new TeamDto()
                {
                    Id = x.Team.Id,
                    Name = x.Team.Name,
                    AvatarUrl = x.Team.AvatarUrl
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

    public async Task<GlobalResponse<UserDto>> PostUserDetails(Guid id, PostUserDetailsDto postDetailsCommand)
    {
        var user = await _repository.Users.FindAsync(id);
        if (user == null) return new GlobalResponse<UserDto>(false, "delete user failed", errors: [$"user with id{id} not found"]);

        user.Profession = postDetailsCommand.Profession;
        user.FullName = postDetailsCommand.FullName;
        user.UserName = postDetailsCommand.UserName;

        var validationResult = await new UserValidator().ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            return new GlobalResponse<UserDto>(false, "update user failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        if (postDetailsCommand.Avatar != null)
        {
            var uploadResponse = await _r2Cloud.UploadFileAsync(postDetailsCommand.Avatar, $"u_img_{user.UserName}");
            if (uploadResponse.IsSuccess)
            {
                user.AvatarUrl = uploadResponse.Data.Url;
            }
        }
        else
        {
            user.AvatarUrl = $"https://eu.ui-avatars.com/api/?name={user.UserName}&size=250";
        }

        user.OnBoardingProgress = OnBoardingProgress.Details;

        await _repository.SaveChangesAsync();

        return new GlobalResponse<UserDto>(true, "post details success");
    }

    public async Task<GlobalResponse<UserDto>> PostUserSkills(Guid id, List<string> postSkillsCommand)
    {
        var user = await _repository.Users.FindAsync(id);
        if (user == null) return new GlobalResponse<UserDto>(false, "delete user failed", errors: [$"user with id{id} not found"]);

        user.Skills = postSkillsCommand;
        await _repository.SaveChangesAsync();

        return new GlobalResponse<UserDto>(true, "post skills success");

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
