using System.Security.Permissions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.Auth;
using synthesis.api.Features.Project;
using synthesis.api.Features.User;
using synthesis.api.Helpers;
using synthesis.api.Mappings;
using synthesis.api.Services.Email;
using Synthesis.Api.Services.BlobStorage;

namespace synthesis.api.Features.Teams;

public interface ITeamService
{
    Task<GlobalResponse<TeamDto>> CreateTeam(Guid userId, CreateTeamDto createCommand);
    Task<GlobalResponse<MemberDto>> AddMember(Guid id, Guid userId);
    Task<GlobalResponse<MemberDto>> RemoveMember(Guid id, Guid userId);
    Task<GlobalResponse<TeamDto>> GetTeamById(Guid id);
    Task<GlobalResponse<TeamDto>> GetTeamWithResourcesById(Guid id);
    Task<GlobalResponse<List<MemberDto>>> GetTeamMembers(Guid id);
    Task<GlobalResponse<List<ProjectDto>>> GetTeamProjects(Guid id);
    Task<GlobalResponse<TeamDto>> UpdateTeam(Guid id, UpdateTeamDto updateCommand);
    Task<GlobalResponse<TeamDto>> PatchTeam(Guid id, UpdateTeamDto patchCommand);
    Task<GlobalResponse<TeamDto>> DeleteTeam(Guid id);
    Task<GlobalResponse<MemberDto>> JoinTeam(Guid userId, string code);
    Task<GlobalResponse<string>> InviteTeamMembers(Guid id, List<MemberInviteDto> invites);


}

public class TeamService : ITeamService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;
    private readonly R2CloudStorage _r2Cloud;
    private readonly IJwtTokenManager _jwtManager;
    private readonly IEmailService _emailService;

    public TeamService(RepositoryContext repository, IMapper mapper, R2CloudStorage r2Cloud, IJwtTokenManager jwtManager, IEmailService emailService)
    {
        _repository = repository;
        _mapper = mapper;
        _r2Cloud = r2Cloud;
        _jwtManager = jwtManager;
        _emailService = emailService;
    }

    public async Task<GlobalResponse<TeamDto>> CreateTeam(Guid userId, CreateTeamDto createCommand)
    {
        var user = await _repository.Users.FindAsync(userId);

        if (user == null)
        {
            return new GlobalResponse<TeamDto>(false, "create team failed", errors: [$"user with id: {userId} not found"]);
        }

        var slugExists = await _repository.Teams.AnyAsync(t => t.Slug.ToLower() == t.Slug.ToLower());

        if (slugExists)
        {
            return new GlobalResponse<TeamDto>(false, "create team failed", errors: ["the team slug is already taken"]);
        }

        var team = new TeamModel()
        {
            CreatedOn = DateTime.UtcNow,
            Name = createCommand.Name,
            Slug = createCommand.Slug,
            SeatsAvailable = 3,
        };

        var validationResult = new TeamValidator().Validate(team);

        if (!validationResult.IsValid)
        {
            return new GlobalResponse<TeamDto>(false, "create team failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        if (createCommand.Avatar != null)
        {
            var uploadResponse = await _r2Cloud.UploadFileAsync(createCommand.Avatar, $"t_img_{team.Slug}");
            if (uploadResponse.IsSuccess)
            {
                team.AvatarUrl = uploadResponse.Data.Url;
            }
        }
        else
        {
            team.AvatarUrl = $"https://ui-avatars.com/api/?name={team.Name}&background=random&size=250"; ;
        }

        var member = new MemberModel()
        {
            User = user,
            Roles = [MemberRole.owner.GetDisplayName()],
            JoinedOn = DateTime.UtcNow
        };

        team.Members = [member];

        await _repository.Teams.AddAsync(team);

        await _repository.SaveChangesAsync();

        var teamToReturn = new TeamDto()
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            Slug = team.Slug,
            AvatarUrl = team.AvatarUrl
        };


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
            return new GlobalResponse<MemberDto>(false, "add member to team failed", errors: [$"member with id {userId} is already a member of the team"]);
        }

        var member = new MemberModel()
        {
            User = user,
            Team = team,
            JoinedOn = DateTime.UtcNow
        };

        await _repository.Members.AddAsync(member);
        await _repository.SaveChangesAsync();


        var memberToReturn = new MemberDto()
        {
            Id = member.Id,
            User = new UserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Profession = user.Profession,
                Skills = user.Skills,
                AvatarUrl = user.AvatarUrl,
            },
            Roles = member.Roles,
            JoinedOn = member.JoinedOn
        };

        return new GlobalResponse<MemberDto>(true, "add member to team success", value: memberToReturn);
    }

    public async Task<GlobalResponse<MemberDto>> RemoveMember(Guid id, Guid userId)
    {
        var team = await _repository.Teams.FindAsync(id);

        if (team == null)
        {
            return new GlobalResponse<MemberDto>(false, "remove member from team failed", errors: [$"team with id {id} not found"]);
        }

        var user = await _repository.Users.AnyAsync(u => u.Id == userId);

        if (!user)
        {
            return new GlobalResponse<MemberDto>(false, "remove member from team failed", errors: [$"user with id {userId} not found"]);
        }

        var member = await _repository.Members.FirstOrDefaultAsync(m => m.UserId == userId && m.TeamId == id);
        if (member == null)
        {
            return new GlobalResponse<MemberDto>(false, "remove member from team failed", errors: [$"user with id: {userId} is not a member of the team"]);
        }

        _repository.Members.Remove(member);
        await _repository.SaveChangesAsync();


        return new GlobalResponse<MemberDto>(true, "remove member from team success");
    }

    public async Task<GlobalResponse<TeamDto>> GetTeamById(Guid id)
    {
        var team = await _repository.Teams
        .Where(t => t.Id == id)
        .Select(x => new TeamDto
        {
            Id = x.Id,
            Name = x.Name,
            AvatarUrl = x.AvatarUrl,
            Slug = x.Slug
        }
        ).FirstOrDefaultAsync();

        if (team == null)
        {
            return new GlobalResponse<TeamDto>(true, "get team failed", errors: [$"team with id: {id} not found"]);
        }


        return new GlobalResponse<TeamDto>(true, "get team success", value: team);
    }

    public async Task<GlobalResponse<TeamDto>> GetTeamWithResourcesById(Guid id)
    {
        var team = await _repository.Teams.Where(t => t.Id == id)
        .Select(t => new TeamDto()
        {
            Id = t.Id,
            Name = t.Name,
            AvatarUrl = t.AvatarUrl,
            Slug = t.Slug,
            Members = t.Members.Select(m => new MemberDto
            {
                Id = m.Id,
                User = new UserDto()
                {
                    Id = m.User.Id,
                    FullName = m.User.FullName,
                    UserName = m.User.UserName,
                    AvatarUrl = m.User.AvatarUrl,
                    Profession = m.User.Profession,
                    Skills = m.User.Skills,
                    Email = m.User.Email
                },
                Roles = m.Roles,
                JoinedOn = m.JoinedOn
            }).ToList(),
            Projects = t.Projects.Select(p => new ProjectDto
            {
                Id = p.Id

            }).ToList()
        }).
        FirstOrDefaultAsync();

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
        .Select(m => new MemberDto()
        {
            Id = m.Id,
            User = new UserDto()
            {
                Id = m.User.Id,
                FullName = m.User.FullName,
                UserName = m.User.UserName,
                Email = m.User.Email,
                Profession = m.User.Profession,
                Skills = m.User.Skills,
                AvatarUrl = m.User.AvatarUrl
            },
            Roles = m.Roles,
            JoinedOn = m.JoinedOn
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

    public async Task<GlobalResponse<string>> InviteTeamMembers(Guid id, List<MemberInviteDto> invites)
    {

        var team = await _repository.Teams.Where(t => t.Id == id)
        .Select(x => new TeamModel { Id = x.Id, Name = x.Name }).FirstOrDefaultAsync();

        if (team == null) return new GlobalResponse<string>(false, "invite members failed", errors: [$"team with id: {id} not found"]);


        var invitations = invites.Select(x => new InviteModel
        {
            TeamId = id,
            Code = CodeGenerator.GenerateCode(),
            Email = x.Email,
            Role = x.Role,
            InvitedOn = DateTime.UtcNow,
            Accepted = false,
        }).ToList();

        await _repository.Invites.AddRangeAsync(invitations);

        await _repository.SaveChangesAsync();

        var recepients = invitations.Select(x => new InviteRecepientDto { Email = x.Email, Code = x.Code }).ToList();

        var sendEmailResponses = new List<GlobalResponse<string>>();

        foreach (var recepient in recepients)
        {
            var sendEmailResponse = await _emailService.SendTeamInvitationEmail(team.Name, recepient);
            sendEmailResponses.Add(sendEmailResponse);
        }

        if (sendEmailResponses.Any(r => r.IsSuccess == false))
        {
            return new GlobalResponse<string>(true, "invites were sent but some were unsuccessful", recepients[0].Code);
        }

        return new GlobalResponse<string>(true, "member invites success");


    }

    public async Task<GlobalResponse<MemberDto>> JoinTeam(Guid userId, string code)
    {
        
        var invite = await _repository.Invites.Include(i => i.Team).FirstOrDefaultAsync(i => i.Code == code);

        if (invite == null) return new GlobalResponse<MemberDto>(false, "join team failed", errors: [$"invalide invite code"]);


        var memberExists = await _repository.Members.AnyAsync(m => m.UserId == userId && m.TeamId == invite.TeamId);
        if (memberExists)
        {
            return new GlobalResponse<MemberDto>(false, "add member to team failed", errors: [$"member with id {userId} is already a member of the team"]);
        }

        var member = new MemberModel()
        {
            UserId = userId,
            TeamId = invite.TeamId,
            Roles = [invite.Role],
            JoinedOn = DateTime.UtcNow
        };

        await _repository.Members.AddAsync(member);
        await _repository.SaveChangesAsync();

        var memberToReturn = new MemberDto()
        {
            Id = member.Id,
            Team = new TeamDto
            {
                Id = invite.Team.Id,
                Slug = invite.Team.Slug,
                Name = invite.Team.Name,
                Description = invite.Team.Description,
                AvatarUrl = invite.Team.AvatarUrl
            },
            Roles = member.Roles,
            JoinedOn = member.JoinedOn
        };

        return new GlobalResponse<MemberDto>(true, "add member to team success", value: memberToReturn);


    }
}