using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.Feature;
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

    Task<GlobalResponse<ProjectDto>> GetProjectWithResourcesById(Guid id);

    Task<GlobalResponse<PlanDto>> GetGeneratedPrePlan(Guid planId);

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
        var prePlan = await _repository.PrePlans.FindAsync(planId);


        if (prePlan == null)
        {
            return new GlobalResponse<ProjectDto>(false, "create project failed", errors: [$"plan with id: {planId} not found"]);
        }

        var generatedPreplan = JsonSerializer.Deserialize<GeneratedPrePlanDto>(prePlan.Plan);

        if (generatedPreplan == null) return new GlobalResponse<ProjectDto>(false, "create project failed", errors: [$"plan was not generated successfully"]);

        var teamMemberIds = _repository.Members.Where(m => m.TeamId == prePlan.TeamId).Select(x => x.Id).ToList();

        var projectId = Guid.NewGuid();
        var project = new ProjectModel()
        {

            Id = projectId,
            TeamId = prePlan.TeamId,
            Name = generatedPreplan.Overview.SuggestedNames.FirstOrDefault().Name,

            Description = generatedPreplan.Overview.Description,

            PrePlanId = planId,



            Features = generatedPreplan.Features.Select(f => new FeatureModel()
            {
                Name = f.Name,
                Description = f.Description,
                Type = (FeatureType)f.Type,
                Tasks = f.Tasks.Select(t => new TaskToDoModel()
                {
                    MemberId = teamMemberIds[new Random().Next(0, teamMemberIds.Count)],
                    AssignedOn = DateTime.UtcNow,
                    State = TaskState.InProgress,
                    ProjectId = projectId,
                    Activity = t.Activity,
                    DueDate = DateTime.UtcNow.AddDays(new Random().Next(1, 14)),
                    Priority = (TaskPriority)t.Priority,
                    CreatedOn = DateTime.UtcNow
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
            PrePlan = new PrePlanMetaData()
            {
                Overview = generatedPreplan.Overview,
                CompetitiveAnalysis = generatedPreplan.CompetitiveAnalysis,
                Branding = generatedPreplan.Branding,
                Technology = generatedPreplan.Technology
            },
            Features = project.Features.Select(f => new FeatureDto()
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                Type = f.Description,
                Tasks = f.Tasks.Select(t => new TaskDto()
                {
                    Id = t.Id,
                    Activity = t.Activity,
                    IsComplete = t.IsComplete,
                    AssignedOn = t.AssignedOn,
                    CreatedOn = t.CreatedOn,
                    DueDate = t.DueDate,
                    Priority = t.Priority.GetDisplayName(),
                    State = t.State.GetDisplayName()
                }).ToList()
            }).ToList(),
        };

        return new GlobalResponse<ProjectDto>(true, "create project success", projectToReturn);
    }


    public async Task<GlobalResponse<string>> GenerateProject(Guid teamId, string prompt)
    {
        /*here i initalize a new planId and a planDto 
        which is to be stored in cache as a placeholder
        for the preplan being generated
        */

        var teamMembers = _repository.Members.Where(m => m.TeamId == teamId).Select(x => new GptTeamMemberDto()
        {
            Id = x.Id,
            Skills = x.User.Skills
        }).ToList();



        var planId = Guid.NewGuid();
        var plan = new PlanDto()
        {
            Id = planId,
            TeamId = teamId,
            Status = PlanStatus.Inprogress,
            IsSuccess = false
        };

        //store the plan placeholder in cache and set an expiration time of 20 minutes
        _cache.SetData(planId.ToString(), plan, DateTimeOffset.UtcNow.AddMinutes(20));

        //i initialize the preplan generation process on a background thread
        _ = Task.Run(async () => await HandleProjectGeneration(teamId, plan.Id, prompt, teamMembers));

        //i return a plan id to the user which they will use to ping the get projects endpoint
        return new GlobalResponse<string>(true, "accepted", value: plan.Id.ToString());

    }

    public async Task<GlobalResponse<PlanDto>> GetGeneratedPrePlan(Guid planId)
    {
        var prePlanIsSaved = await _repository.PrePlans.AnyAsync(p => p.Id == planId);

        if (prePlanIsSaved)
        {
            var savedPreplan = await _repository.PrePlans.FindAsync(planId);

            var savedPreplanDto = new PlanDto()
            {
                Id = savedPreplan.Id,
                TeamId = savedPreplan.TeamId,
                IsSuccess = savedPreplan.IsSuccess,
                Status = savedPreplan.Status,
                Plan = JsonSerializer.Deserialize<GeneratedPrePlanDto>(savedPreplan.Plan)
            };


            return new GlobalResponse<PlanDto>(true, "get preplan success", savedPreplanDto);

        }

        var prePlan = _cache.GetData<PlanDto>(planId.ToString());

        if (prePlan.Status == PlanStatus.Complete)
        {
            prePlan.Plan.Technology = new Technology()
            {
                TechStacks = new List<TechStack>()
                {
                    new TechStack()
                    {
                        Name = ".NET",
                        Description = "A popular web framework for backend dev ",
                        Reason = "highly performant, robust",
                        LogoUrl = "https://icon.icepanel.io/Technology/svg/.NET-core.svg"
                    },
                    new TechStack()
                    {
                        Name = "Postgres",
                        Description = "A flexible Sql Database ",
                        Reason = "highly performant, no-sql support",
                        LogoUrl = "https://icon.icepanel.io/Technology/svg/PostgresSQL.svg"
                    },
                    new TechStack()
                    {
                        Name = "NextJs",
                        Description = "A very popular Front end framework by vercel ",
                        Reason = "highly performance, clean components, multiple libraries",
                        LogoUrl = "https://icon.icepanel.io/Technology/svg/nextjs.svg"
                    },

                }
            };
        }

        if (!prePlan.IsSuccess)
        {
            if (prePlan.Status == PlanStatus.Inprogress)
            {
                return new GlobalResponse<PlanDto>(true, "pending");
            }

            else
            {
                return new GlobalResponse<PlanDto>(false, "get generated project failed", errors: [$"something went wrong"]);

            }
        }

        var prePlanToSave = new PrePlanModel()
        {
            TeamId = prePlan.TeamId,
            Id = planId,
            Plan = JsonSerializer.Serialize(prePlan.Plan),
            IsSuccess = prePlan.IsSuccess,
            Status = prePlan.Status
        };

        await _repository.PrePlans.AddAsync(prePlanToSave);
        await _repository.SaveChangesAsync();

        var prePlanToReturn = new PlanDto()
        {
            Id = prePlanToSave.Id,
            TeamId = prePlanToSave.TeamId,
            Plan = JsonSerializer.Deserialize<GeneratedPrePlanDto>(prePlanToSave.Plan),
            IsSuccess = prePlanToSave.IsSuccess,
            Status = prePlanToSave.Status
        };
        return new GlobalResponse<PlanDto>(true, "get generate  d project success", prePlanToReturn);
    }

    private async Task HandleProjectGeneration(Guid teamId, Guid planId, string prompt, List<GptTeamMemberDto> teamMembers)
    {
        /*intialize the project generation 
        by passing the idea prompt as a parameter
        to the Generate Project method exposed by the GptService*/
        var response = await _gptService.GeneratePreplan(prompt, teamMembers);


        //retrieve the placeholder plandto in cache to be modified
        var plan = _cache.GetData<PlanDto>(planId.ToString());

        //if the response from the project generation process is successful 
        //modify the retrieved placeholder plandto and set the props to indicate 
        //successful generation and write the plan with data to the cache
        if (response.IsSuccess)
        {
            plan.Id = planId;
            plan.TeamId = teamId;
            plan.IsSuccess = true;
            plan.Status = PlanStatus.Complete;
            plan.Plan = response.Data;

            _cache.SetData(planId.ToString(), plan, DateTimeOffset.Now.AddMinutes(20));
            return;
        }

        //else if reponse is not successfull set the plan status to failed
        plan.Status = PlanStatus.Failed;

        //update the cache with the plan with an updated success.
        _cache.SetData(planId.ToString(), plan, DateTimeOffset.Now.AddMinutes(20));

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

    public async Task<GlobalResponse<ProjectDto>> GetProjectWithResourcesById(Guid id)
    {
        var project = await _repository.Projects.Where(p => p.Id == id)
        .Select(p => new ProjectDto()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            AvatarUrl = p.AvatarUrl,
            Features = p.Features.Select(f => new FeatureDto()
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                Type = f.Description,
                Tasks = f.Tasks.Select(t => new TaskDto()
                {
                    Id = t.Id,
                    Activity = t.Activity,
                    IsComplete = t.IsComplete,
                    AssignedOn = t.AssignedOn,
                    CreatedOn = t.CreatedOn,
                    DueDate = t.DueDate,
                    Priority = t.Priority.GetDisplayName(),
                    State = t.State.GetDisplayName()
                }).ToList()
            }).ToList(),
            Tasks = p.Tasks.Where(t => t.FeatureId == null).Select(t => new TaskDto()
            {
                Id = t.Id,
                Activity = t.Activity,
                IsComplete = t.IsComplete,
                AssignedOn = t.AssignedOn,
                CreatedOn = t.CreatedOn,
                DueDate = t.DueDate,
                Priority = t.Priority.GetDisplayName(),
                State = t.State.GetDisplayName()
            }).ToList(),
            PrePlan = PrePlanDeserializer.DeserializePrePlanToMetaData(p.PrePlan.Plan)
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