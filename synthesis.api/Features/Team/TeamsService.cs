using AutoMapper;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Team;

public interface ITeamService
{
    Task<GlobalResponse<TeamDto>> CreateTeam(Guid projectId, CreateTeamDto createRequest);
    Task<GlobalResponse<TeamDto>> GetTeamById(Guid id);
    Task<GlobalResponse<List<MemberDto>>> GetTeamMembers(Guid id);
    Task<GlobalResponse<TeamDto>> UpdateTeam(Guid id, UpdateTeamDto updateRequest);
    Task<GlobalResponse<TeamDto>> PatchTeam(Guid id, UpdateTeamDto updateRequest);
    Task<GlobalResponse<TeamDto>> DeleteTeam(Guid id);
    Task<GlobalResponse<TeamDto>> AddTeamMember(Guid id, Guid memberId);
    Task<GlobalResponse<TeamDto>> RemoveTeamMember(Guid id, Guid memberId);

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

    public async Task<GlobalResponse<TeamDto>> CreateTeam(Guid projectId, CreateTeamDto createRequest)
    {
        var projectExists = await _repository.Projects.AnyAsync(p => p.Id == projectId);

        if (!projectExists) return new GlobalResponse<TeamDto>(false, "create team failed", errors: [$"project with id: {projectId} not found"]);

        var team = _mapper.Map<TeamModel>(createRequest);

        team.ProjectId = projectId;

        var validationResult = new TeamValidator().Validate(team);
        if (!validationResult.IsValid) return new GlobalResponse<TeamDto>(false, "create team failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.Teams.AddAsync(team);

        await _repository.SaveChangesAsync();

        var teamToReturn = _mapper.Map<TeamDto>(team);

        return new GlobalResponse<TeamDto>(true, "created team successfully", value: teamToReturn);
    }

    public async Task<GlobalResponse<TeamDto>> GetTeamById(Guid id)
    {
        var team = await _repository.Teams.FindAsync(id);

        if (team == null) return new GlobalResponse<TeamDto>(false, "get team failed", errors: [$"team with id:{id} not found"]);

        var teamToReturn = _mapper.Map<TeamDto>(team);

        return new GlobalResponse<TeamDto>(true, "get team success", value: teamToReturn);
    }

    public async Task<GlobalResponse<TeamDto>> UpdateTeam(Guid id, UpdateTeamDto updateRequest)
    {
        var team = await _repository.Teams.FindAsync(id);
        if (team == null) return new GlobalResponse<TeamDto>(false, "update team failed", errors: [$"team with id: {id} not found"]);

        var updatedTeam = _mapper.Map(updateRequest, team);

        var validationResult = new TeamValidator().Validate(updatedTeam);
        if (!validationResult.IsValid) return new GlobalResponse<TeamDto>(false, "update team failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.SaveChangesAsync();

        return new GlobalResponse<TeamDto>(true, "update team success");

    }

    public async Task<GlobalResponse<TeamDto>> PatchTeam(Guid id, UpdateTeamDto patchRequest)
    {
        var team = await _repository.Teams.FindAsync(id);
        if (team == null) return new GlobalResponse<TeamDto>(false, "patch team failed", errors: [$"team with id:{id} not found"]);

        var teamToBePatched = _mapper.Map<UpdateTeamDto>(team);

        var patchedTeamDto = Patcher.Patch(patchRequest, teamToBePatched);

        var patchedTeam = _mapper.Map(patchedTeamDto, team);

        var validationResult = new TeamValidator().Validate(patchedTeam);

        if (!validationResult.IsValid) return new GlobalResponse<TeamDto>(false, "patch team failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.SaveChangesAsync();

        return new GlobalResponse<TeamDto>(true, "patch team success");

    }

    public async Task<GlobalResponse<TeamDto>> DeleteTeam(Guid id)
    {
        var team = await _repository.Teams.FindAsync(id);

        if (team == null) return new GlobalResponse<TeamDto>(false, "delete team failed", errors: [$"team with id: {id} not found"]);

        _repository.Teams.Remove(team);

        await _repository.SaveChangesAsync();

        return new GlobalResponse<TeamDto>(true, "delete team success");
    }

    public async Task<GlobalResponse<List<MemberDto>>> GetTeamMembers(Guid id)
    {
        var teamExists = await _repository.Teams.AnyAsync(t => t.Id == id);

        if (!teamExists)
        {
            return new GlobalResponse<List<MemberDto>>(false, "get team members failed", errors: [$"team with id: {id} not found"]);
        }

        var members = await _repository.Teams.Where(t => t.Id == id)
            .SelectMany(t => t.Developers)
            .Select(x => new MemberDto()
            {
                Id = x.Id,
                User = new UserDto()
                {
                    Id = x.User.Id,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Username = x.User.UserName,
                    AvatarUrl = x.User.AvatarUrl,
                    Email = x.User.AvatarUrl
                },

            })
            .ToListAsync();


        return new GlobalResponse<List<MemberDto>>(true, "get team members success", value: members);
    }

    public async Task<GlobalResponse<TeamDto>> AddTeamMember(Guid id, Guid memberId)
    {
        var team = await _repository.Teams.FindAsync(id);
        if (team == null) return new GlobalResponse<TeamDto>(false, "add developer to team failed", errors: [$"team with id:{id} not found"]);

        var member = await _repository.Members.FindAsync(memberId);
        if (member == null) return new GlobalResponse<TeamDto>(false, "add developer to team failed", errors: [$"member with id: {id} not found"]);

        member.Teams = [team];

        await _repository.SaveChangesAsync();

        return new GlobalResponse<TeamDto>(true, "add developer to team success");
    }

    public async Task<GlobalResponse<TeamDto>> RemoveTeamMember(Guid id, Guid memberId)
    {
        var team = await _repository.Teams.FindAsync(id);
        if (team == null) return new GlobalResponse<TeamDto>(false, "remove developer from team failed", errors: [$"team with id:{id} not found"]);

        var member = await _repository.Members.Where(m => m.Id == memberId).Include(m => m.Teams).SingleOrDefaultAsync();
        if (member == null) return new GlobalResponse<TeamDto>(false, "remove developer from team failed", errors: [$"member with id: {id} not found"]);

        if (member.Teams != null) member.Teams.Remove(team);

        await _repository.SaveChangesAsync();

        return new GlobalResponse<TeamDto>(true, "remove developer from team success");
    }

}