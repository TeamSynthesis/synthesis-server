using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.Project;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Teams;

public interface ITeamService
{
    Task<GlobalResponse<TeamDto>> CreateTeam(Guid userId, CreateTeamDto createCommand);
    Task<GlobalResponse<MemberDto>> AddMember(Guid id, Guid userId);
    Task<GlobalResponse<TeamDto>> GetTeamById(Guid id);
    Task<GlobalResponse<TeamDto>> GetTeamWithResourcesById(Guid id);
    Task<GlobalResponse<List<MemberDto>>> GetTeamMembers(Guid id);
    Task<GlobalResponse<List<ProjectDto>>> GetTeamProjects(Guid id);
    Task<GlobalResponse<TeamDto>> UpdateTeam(Guid id, UpdateTeamDto updateCommand);
    Task<GlobalResponse<TeamDto>> PatchTeam(Guid id, UpdateTeamDto patchCommand);
    Task<GlobalResponse<TeamDto>> DeleteTeam(Guid id);

}

public class TeamService : ITeamService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;

    public TeamService(RepositoryContext repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<GlobalResponse<TeamDto>> CreateTeam(Guid userId, CreateTeamDto createCommand)
    {
        var user = await _repository.Users.FindAsync(userId);

        if (user == null)
        {
            return new GlobalResponse<TeamDto>(false, "create team failed", errors: [$"user with id: {userId} not found"]);
        }

        var team = new TeamModel()
        {
            Name = createCommand.Name,
            LogoUrl = createCommand.LogoUrl ?? $"https://eu.ui-avatars.com/api/?name={createCommand.Name}&size=250"
        };

        var validationResult = new TeamValidator().Validate(team);

        if (!validationResult.IsValid)
        {
            return new GlobalResponse<TeamDto>(false, "create team failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var member = new MemberModel()
        {
            User = user,
            Roles = [UserRoles.Owner]
        };

        team.Members = [member];

        await _repository.Teams.AddAsync(team);

        await _repository.SaveChangesAsync();

        var teamToReturn = _mapper.Map<TeamDto>(team);

        return new GlobalResponse<TeamDto>(true, "team created", value: teamToReturn);

    }

    public async Task<GlobalResponse<MemberDto>> AddMember(Guid id, Guid userId)
    {
        var team = await _repository.Teams.FindAsync(id);

        if (team == null)
        {
            return new GlobalResponse<MemberDto>(false, "add member to team failed", errors: [$"team with id {id} not found"]);
        }

        var user = await _repository.Users.FindAsync(userId);

        if (user == null)
        {
            return new GlobalResponse<MemberDto>(false, "add member to team failed", errors: [$"user with id {userId} not found"]);
        }

        var memberExists = await _repository.Members.AnyAsync(m => m.UserId == userId && m.TeamId == id);
        if (memberExists)
        {
            return new GlobalResponse<MemberDto>(false, "add member to team failed", errors: [$"member with id {userId} is already part of the team"]);
        }

        var member = new MemberModel()
        {
            User = user,
            Team = team
        };

        await _repository.Members.AddAsync(member);
        await _repository.SaveChangesAsync();


        var memberToReturn = _mapper.Map<MemberDto>(member);

        return new GlobalResponse<MemberDto>(true, "add member to team success", value: memberToReturn);
    }

    public async Task<GlobalResponse<TeamDto>> GetTeamById(Guid id)
    {
        var team = await _repository.Teams
        .Where(t => t.Id == id)
        .Select(x => new TeamDto
        {
            Id = x.Id,
            Name = x.Name,
            LogoUrl = x.LogoUrl
        }
        ).SingleOrDefaultAsync();

        if (team == null)
        {
            return new GlobalResponse<TeamDto>(true, "get team failed", errors: [$"team with id: {id} not found"]);
        }


        return new GlobalResponse<TeamDto>(true, "get team success", value: team);
    }

    public async Task<GlobalResponse<TeamDto>> GetTeamWithResourcesById(Guid id)
    {
        var team = await _repository.Teams.Where(t => t.Id == id)
        .Select(org => new TeamDto()
        {
            Id = org.Id,
            Name = org.Name,
            LogoUrl = org.LogoUrl,
            Members = org.Members.Select(x => new MemberDto
            {
                Id = x.Id,
                User = new UserDto()
                {
                    Id = x.User.Id,
                    Username = x.User.UserName,
                    AvatarUrl = x.User.AvatarUrl,
                    Email = x.User.AvatarUrl

                },
                Roles = x.Roles
            }).ToList(),
            Projects = org.Projects.Select(x => new ProjectDto
            {
                Id = x.Id

            }).ToList()
        }).
        SingleOrDefaultAsync();

        if (team == null)
        {
            return new GlobalResponse<TeamDto>(true, "get team failed", errors: [$"team with id: {id} not found"]);
        }


        return new GlobalResponse<TeamDto>(true, "get team success", value: team);

    }

    public async Task<GlobalResponse<List<MemberDto>>> GetTeamMembers(Guid id)
    {
        var organisationExists = await _repository.Teams.AnyAsync(org => org.Id == id);

        if (!organisationExists)
        {
            return new GlobalResponse<List<MemberDto>>(false, "get members failed", errors: [$"organisation with id: {id} not found"]);
        }

        var members = await _repository.Members
        .Where(m => m.TeamId == id)
        .Select(x => new MemberDto()
        {
            Id = x.Id,
            User = new UserDto()
            {
                Id = x.User.Id,
                Username = x.User.UserName,
                Email = x.User.Email,
                AvatarUrl = x.User.AvatarUrl
            },
            Roles = x.Roles
        }).ToListAsync();


        return new GlobalResponse<List<MemberDto>>(true, "get members success", value: members);

    }

    public async Task<GlobalResponse<List<ProjectDto>>> GetTeamProjects(Guid id)
    {
        var team = await _repository.Teams.AnyAsync(t => t.Id == id);

        if (!team)
        {
            return new GlobalResponse<List<ProjectDto>>(false, "get projects failed", errors: [$"team with id{id} not found"]);
        }

        var projects = await _repository.Projects
            .Where(p => p.TeamId == id)
            .Select(x => new ProjectDto
            {
                Id = x.Id
            }).ToListAsync();


        return new GlobalResponse<List<ProjectDto>>(true, "get projects success", value: projects);

    }

    public async Task<GlobalResponse<TeamDto>> UpdateTeam(Guid id, UpdateTeamDto updateCommand)
    {
        var team = await _repository.Teams.FindAsync(id);

        if (team == null)
        {
            return new GlobalResponse<TeamDto>(false, "update team failed", errors: [$"team with id: {id} not found"]);
        }

        var updatedTeam = _mapper.Map(updateCommand, team);

        var validationResult = new TeamValidator().Validate(updatedTeam);

        if (!validationResult.IsValid)
        {
            return new GlobalResponse<TeamDto>(false, "update team failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new GlobalResponse<TeamDto>(true, "update team success");

    }

    public async Task<GlobalResponse<TeamDto>> PatchTeam(Guid id, UpdateTeamDto patchCommand)
    {
        var team = await _repository.Teams.FindAsync(id);
        if (team == null) return new GlobalResponse<TeamDto>(false, "delete team failed", errors: [$"team with id{id} not found"]);

        var teamToBePatched = _mapper.Map<UpdateTeamDto>(team);

        var patchedTeamDto = Patcher.Patch(patchCommand, teamToBePatched);

        var patchedTeam = _mapper.Map(patchedTeamDto, team);

        var validationResult = new TeamValidator().Validate(patchedTeam);
        if (!validationResult.IsValid)
        {
            return new GlobalResponse<TeamDto>(false, "patch team failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new GlobalResponse<TeamDto>(true, "patch team success");
    }

    public async Task<GlobalResponse<TeamDto>> DeleteTeam(Guid id)
    {
        var team = await _repository.Teams.FindAsync(id);

        if (team == null)
        {
            return new GlobalResponse<TeamDto>(false, "update team failed", errors: [$"team with id: {id} not found"]);
        }

        _repository.Teams.Remove(team);

        await _repository.SaveChangesAsync();

        return new GlobalResponse<TeamDto>(true, "delete team success");
    }
}