
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Mappings;
using synthesis.api.Services.OpenAi;
using synthesis.api.Services.OpenAi.Dtos;

namespace synthesis.api.Features.Project;

public interface IProjectService
{
    Task<GlobalResponse<ProjectDto>> CreateProject(Guid teamId, CreateProjectDto createCommand);

    Task<GlobalResponse<GptProjectDto>> GenerateProject(string prompt);

    Task<GlobalResponse<ProjectDto>> UpdateProject(Guid id, UpdateProjectDto updateCommand);

    Task<GlobalResponse<ProjectDto>> GetProjectById(Guid id);

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

        project.AvatarUrl = $"https://eu.ui-avatars.com/api/?name={project.Name}&size=250";

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

    public async Task<GlobalResponse<GptProjectDto>> GenerateProject(string prompt)
    {

        var projectDto = await _gptService.GenerateProject(prompt);
        if (projectDto == null)
            return new GlobalResponse<GptProjectDto>(false, "project generation failed", errors: [$"something went wrong"]);

        return new GlobalResponse<GptProjectDto>(true, "success", value: projectDto);
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



}