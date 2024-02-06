using Azure;
using synthesis.api.Data.Repository;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Team;

public interface ITeamService
{
    public Task<GlobalResponse<TeamDto>> CreateTeam(Guid projectId, CreateTeamDto createRequest);
}
public class TeamsService
{
    private readonly RepositoryContext _repository;
    public TeamsService(RepositoryContext repository)
    {
        _repository = repository;
    }



}