using AutoMapper;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Mappings;
using synthesis.api.Services.Cache;
using synthesis.api.Services.OpenAi;
using synthesis.api.Services.OpenAi.Dtos;

namespace synthesis.api.Features.Project;

public interface IProjectService
{
    Task<GlobalResponse<ProjectDto>> CreateProject(Guid teamId, CreateProjectDto createCommand);

    Task<GlobalResponse<string>> GenerateProject(string prompt);
    Task<GlobalResponse<ProjectDto>> CreateAiGeneratedProject(Guid teamId, GptProjectDto gptProjectDto);
     Task<GlobalResponse<ProjectDto>> UpdateProject(Guid id, UpdateProjectDto updateCommand);

    Task<GlobalResponse<ProjectDto>> GetProjectById(Guid id);
    GlobalResponse<GptProjectDto> GetGeneratedProject(string processId);

    Task<GlobalResponse<ProjectDto>> DeleteProject(Guid id);
}

public class ProjectService : IProjectService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;
    private readonly IChatGptService _gptService;
    private readonly ICacheService _cache;

    public ProjectService(RepositoryContext repository, IMapper mapper, IChatGptService gptService, ICacheService cache)
    {
        _repository = repository;
        _mapper = mapper;
        _gptService = gptService;
        _cache = cache;
    }

    public async Task<GlobalResponse<ProjectDto>> CreateProject(Guid teamId, CreateProjectDto createCommand)
    {
        var teamExists = await _repository.Teams.AnyAsync(t => t.Id == teamId);
        if (!teamExists) return new GlobalResponse<ProjectDto>(false, "create project failed", errors: [$"team with id: {teamId} not found"]);


        var project = new ProjectModel()
        {
            TeamId = teamId,

            Name = createCommand.Name,
            Description = createCommand.Description,
            CreatedOn = DateTime.UtcNow
        };

        var validationResult = await new ProjectValidator().ValidateAsync(project);
        if (!validationResult.IsValid)
        {
            return new GlobalResponse<ProjectDto>(false, "create project failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        project.AvatarUrl = $"https://ui-avatars.com/api/?name={project.Name}&background=random&size=250"; ;

        await _repository.Projects.AddAsync(project);
        await _repository.SaveChangesAsync();

        var projectToReturn = new ProjectDto()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            AvatarUrl = project.AvatarUrl
        };

        return new GlobalResponse<ProjectDto>(true, "create project success", projectToReturn);
    }

    public async Task<GlobalResponse<ProjectDto>> CreateAiGeneratedProject(Guid teamId, GptProjectDto gptProjectDto)
    {
        var teamExists = await _repository.Teams.AnyAsync(t => t.Id == teamId);
        if (!teamExists) return new GlobalResponse<ProjectDto>(false, "create project failed", errors: [$"team with id: {teamId} not found"]);


        var project = new ProjectModel()
        {
            TeamId = teamId,

            Name = gptProjectDto.Overview.SuggestedNames.FirstOrDefault().Name,
            Description = gptProjectDto.Overview.Description,
            ProjectMetadata = new ProjectMetadata()
            {
                Overview = gptProjectDto.Overview,
                CompetitiveAnalysis = gptProjectDto.CompetitiveAnalysis,
                Branding = gptProjectDto.Branding
            },

            Features = gptProjectDto.Features.Select(f => new FeatureModel()
            {
                Name = f.Name,
                Description = f.Description,
                Type = (FeatureType)f.Type,
                Tasks = f.Tasks.Select(t => new TaskToDoModel()
                {
                    Activity = t.Activity,
                    Priority = (TaskPriority)t.Priority,

                }).ToList(),
            }).ToList(),

            CreatedOn = DateTime.UtcNow
        };



        project.AvatarUrl = $"https://ui-avatars.com/api/?name={project.Name}&background=random&size=250"; ;

        await _repository.Projects.AddAsync(project);
        await _repository.SaveChangesAsync();

        var projectToReturn = new ProjectDto()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            AvatarUrl = project.AvatarUrl,
            Metadata = project.ProjectMetadata
        };

        return new GlobalResponse<ProjectDto>(true, "create project success", projectToReturn);
    }




    public async Task<GlobalResponse<string>> GenerateProject(string prompt)
    {
        var processId = Guid.NewGuid().ToString();
        var pendingResponse = new GlobalResponse<GptProjectDto>(true, "pending");

        _cache.SetData(processId, pendingResponse, DateTimeOffset.UtcNow.AddMinutes(10));

        _ = Task.Run(async () => await HandleProjectGeneration(processId, prompt));

        return new GlobalResponse<string>(true, "accepted", value: processId);

    }

    public async Task<GlobalResponse<ProjectDto>> UpdateProject(Guid id, UpdateProjectDto updateCommand)
    {
        var project = await _repository.Projects.FindAsync(id);
        if (project == null)
            return new GlobalResponse<ProjectDto>(false, "update project failed", errors: [$"project with id: {id} not found"]);

        project.Name = updateCommand.Name;
        project.Description = updateCommand.Description;

        var validationResult = await new ProjectValidator().ValidateAsync(project);
        if (!validationResult.IsValid)
        {
            return new GlobalResponse<ProjectDto>(false, "update project failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();



        return new GlobalResponse<ProjectDto>(true, "update project success");
    }

    public async Task<GlobalResponse<ProjectDto>> GetProjectById(Guid id)
    {
        var project = await _repository.Projects.Where(p => p.Id == id)
        .Select(p => new ProjectDto()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            AvatarUrl = p.AvatarUrl,
            CreatedOn = p.CreatedOn

        }).FirstOrDefaultAsync();

        if (project == null) return new GlobalResponse<ProjectDto>(false, "get project failed", errors: [$"project with id: {id} not found"]);


        return new GlobalResponse<ProjectDto>(true, "get project success", project);
    }

    public async Task<GlobalResponse<ProjectDto>> DeleteProject(Guid id)
    {
        var project = await _repository.Projects.FindAsync(id);

        if (project == null) return new GlobalResponse<ProjectDto>(false, "delete project failed", errors: [$"project with id: {id} not found"]);

        _repository.Projects.Remove(project);
        await _repository.SaveChangesAsync();

        return new GlobalResponse<ProjectDto>(true, "delete project success");
    }

    public GlobalResponse<GptProjectDto> GetGeneratedProject(string processId)
    {
        var response = _cache.GetData<GlobalResponse<GptProjectDto>>(processId);
        if (response.Message == null) return new GlobalResponse<GptProjectDto>(false, "get generated project failed", errors: [$"process with id: {processId} not found"]);

        return response;
    }

    private async Task HandleProjectGeneration(string processId, string prompt)
    {

        var response = await _gptService.GenerateProject(prompt);

        _cache.SetData(processId, response, DateTimeOffset.UtcNow.AddMinutes(10));
    }

}