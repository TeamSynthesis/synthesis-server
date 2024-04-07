using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication.PgOutput.Messages;
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

    Task<GlobalResponse<string>> GenerateProject(Guid teamId, string prompt);
    Task<GlobalResponse<ProjectDto>> CreateAiGeneratedProject(Guid planId);
    Task<GlobalResponse<ProjectDto>> UpdateProject(Guid id, UpdateProjectDto updateCommand);

    Task<GlobalResponse<ProjectDto>> GetProjectById(Guid id);
    Task<GlobalResponse<PlanDto>> GetGeneratedProject(Guid planId);

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
            AvatarUrl = $"https://ui-avatars.com/api/?name={createCommand.Name}&background=random&size=250",
            Name = createCommand.Name,
            Description = createCommand.Description,
            CreatedOn = DateTime.UtcNow
        };

        var validationResult = await new ProjectValidator().ValidateAsync(project);
        if (!validationResult.IsValid)
        {
            return new GlobalResponse<ProjectDto>(false, "create project failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }


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

    public async Task<GlobalResponse<ProjectDto>> CreateAiGeneratedProject(Guid planId)
    {
        var plan = await _repository.Plans.FindAsync(planId);
        var gptProject = plan.Project;


        if (plan == null) return new GlobalResponse<ProjectDto>(false, "create project failed", errors: [$"plan with id: {plan} not found"]);


        var project = new ProjectModel()
        {
            TeamId = plan.TeamId,

            Name = gptProject.Overview.SuggestedNames.FirstOrDefault().Name,
            Description = gptProject.Overview.Description,
            ProjectMetadata = new ProjectMetadata()
            {
                Overview = gptProject.Overview,
                CompetitiveAnalysis = gptProject.CompetitiveAnalysis,
                Branding = gptProject.Branding
            },

            Features = gptProject.Features.Select(f => new FeatureModel()
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

    public async Task<GlobalResponse<string>> GenerateProject(Guid teamId, string prompt)
    {
        var planId = Guid.NewGuid();

        _ = Task.Run(async () => await HandleProjectGeneration(teamId, planId, prompt));

        return new GlobalResponse<string>(true, "accepted", value: planId.ToString());

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

    public async Task<GlobalResponse<PlanDto>> GetGeneratedProject(Guid planId)
    {
        var plan = await _repository.Plans.AsNoTracking().Where(p => p.Id == planId).Select(x => new PlanDto
        {
            Id = x.Id,
            Plan = x.Project,
            IsSuccess = x.IsSuccess
        }).FirstOrDefaultAsync();

        if (plan == null) return new GlobalResponse<PlanDto>(false, "get generated project failed", errors: [$"plan with id: {planId} not found"]);

        if (!plan.IsSuccess) return new GlobalResponse<PlanDto>(false, "get generated project failed", errors: [$"an error occured whilst we were making your pre-plan"]);

        return new GlobalResponse<PlanDto>(true, "get generated project success", plan);
    }

    private async Task HandleProjectGeneration(Guid teamId, Guid planId, string prompt)
    {

        var response = await _gptService.GenerateProject(prompt);

        await _repository.Plans.AddAsync(new PlanModel() { Id = planId, TeamId = teamId, Project = response.Data, IsSuccess = response.IsSuccess });
    }

}