using synthesis.api.Mappings;

namespace synthesis.api.Features.Project;

public interface IProjectService
{
    Task<Response<ProjectDto>> CreateProjectDto(Guid organisationId, Guid memberId);
}
public class ProjectService
{

}