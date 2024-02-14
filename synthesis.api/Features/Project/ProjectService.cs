
using AutoMapper;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Mappings;
using synthesis.api.Services.OpenAi;
using System.Text.Json;

namespace synthesis.api.Features.Project;

public interface IProjectService
{
    Task<GlobalResponse<ProjectDto>> CreateProject(Guid organisationId, Guid memberId, CreateProjectDto createRequest);

    Task<GlobalResponse<GeneratedProjectDto>> GenerateProject(string prompt);

    Task<GlobalResponse<ProjectModel>> GetProjectById(Guid id);

    Task<GlobalResponse<ProjectDto>> DeleteProject(Guid id);
}

public class ProjectService : IProjectService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;
    private readonly IChatGptService _gptService;

    public ProjectService(RepositoryContext repository, IMapper mapper, IChatGptService gptService)
    {
        _repository = repository;
        _mapper = mapper;
        _gptService = gptService;
    }

    public Task<GlobalResponse<ProjectDto>> CreateProject(Guid organisationId, Guid memberId, CreateProjectDto createRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<GlobalResponse<GeneratedProjectDto>> GenerateProject(string prompt)
    {
        var gptResponse = await _gptService.GenerateProject(prompt);

        var projectMetaDataResponse = JsonSerializer.Deserialize<ProjectMetadata>(gptResponse.Choices[0].Message.Content);

        return new GlobalResponse<GeneratedProjectDto>(true, "success", value: new GeneratedProjectDto { Metadata = projectMetaDataResponse });
    }


    public async Task<GlobalResponse<ProjectModel>> GetProjectById(Guid id)
    {
        var project = await _repository.Projects.FindAsync(id);
        return new GlobalResponse<ProjectModel>(true, "success", value: project);
    }

    public Task<GlobalResponse<ProjectDto>> DeleteProject(Guid id)
    {
        throw new NotImplementedException();
    }



}