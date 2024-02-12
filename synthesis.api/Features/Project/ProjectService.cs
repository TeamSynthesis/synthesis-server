
using AutoMapper;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Project;

public interface IProjectService
{
    Task<GlobalResponse<ProjectDto>> CreateProject(Guid organisationId, Guid memberId, CreateProjectDto createRequest);

    Task<GlobalResponse<ProjectModel>> GetProjectById(Guid id);

    Task<GlobalResponse<ProjectDto>> UpdateProject(Guid id, UpdateProjectDto updateRequest);

    Task<GlobalResponse<ProjectDto>> PatchProject(Guid id, UpdateProjectDto patchRequest);

    Task<GlobalResponse<ProjectDto>> DeleteProject(Guid id);
}

public class ProjectService : IProjectService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;

    public ProjectService(RepositoryContext repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Task<GlobalResponse<ProjectDto>> CreateProject(Guid organisationId, Guid memberId, CreateProjectDto createRequest)
    {
        throw new NotImplementedException();
    }

    public Task<GlobalResponse<ProjectDto>> DeleteProject(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<GlobalResponse<ProjectModel>> GetProjectById(Guid id)
    {
        var project = await _repository.Projects.FindAsync(id);
        return new GlobalResponse<ProjectModel>(true, "success", value: project);
    }

    public Task<GlobalResponse<ProjectDto>> PatchProject(Guid id, UpdateProjectDto patchRequest)
    {
        throw new NotImplementedException();
    }

    public Task<GlobalResponse<ProjectDto>> UpdateProject(Guid id, UpdateProjectDto updateRequest)
    {
        throw new NotImplementedException();
    }
}