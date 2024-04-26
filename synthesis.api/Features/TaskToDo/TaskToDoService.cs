 using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Mappings;

namespace synthesis.api.Features.TaskToDo;

public interface ITaskToDoService
{
    Task<GlobalResponse<TaskDto>> CreateTask(Guid projectId, CreateTaskDto createCommand);
    Task<GlobalResponse<TaskDto>> GetTaskById(Guid id);
    Task<GlobalResponse<TaskDto>> UpdateTask(Guid id, UpdateTaskDto updateCommand);
    Task<GlobalResponse<TaskDto>> PatchTask(Guid id, UpdateTaskDto patchCommand);
    Task<GlobalResponse<TaskDto>> DeleteTask(Guid id);

}

public class TaskToDoService : ITaskToDoService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;
    public TaskToDoService(RepositoryContext repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<GlobalResponse<TaskDto>> CreateTask(Guid projectId, CreateTaskDto createCommand)
    {
        var project = await _repository.Projects.Select(p => new ProjectModel(){Id = p.Id, TeamId = p.TeamId}).FirstOrDefaultAsync();

        if (project==null) return new GlobalResponse<TaskDto>(false, "create task failed", errors: [$"project with id: {projectId} not found"]);

        var memberExists = false;
        if (createCommand.MemberId != Guid.Empty)
        {
            memberExists = await _repository.Members.AnyAsync(m => m.Id == createCommand.MemberId && m.TeamId == project.TeamId);

            if (!memberExists)
            {
                return new GlobalResponse<TaskDto>(false, "create task failed",
                    errors: [$"member with id: {createCommand.MemberId} not found"]);
            }
        }
        
        var featureExists = false;
        if (createCommand.FeatureId != Guid.Empty)
        {
            featureExists = await _repository.Features.AnyAsync(f => f.Id == createCommand.FeatureId && f.ProjectId == projectId);
          
            if (!featureExists)
            {
                return new GlobalResponse<TaskDto>(false, "create task failed", 
                    errors: [$"feature with id: {createCommand.FeatureId} not found"]);
            }
        }
        
        var task = new TaskToDoModel()
        {
            ProjectId = projectId,
            Activity = createCommand.Activity,
            State = TaskState.Pending,
            Priority = createCommand.Priority,
            CreatedOn = DateTime.UtcNow,
            MemberId = memberExists? createCommand.MemberId: null,
            FeatureId = featureExists? createCommand.FeatureId:null
        };

        var validationResult = new TaskValidator().Validate(task);

        if (!validationResult.IsValid) return new GlobalResponse<TaskDto>(false, "create task failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.Tasks.AddAsync(task);

        await _repository.SaveChangesAsync();

        var taskToReturn = new TaskDto()
        {
            Id = task.Id,
            Activity = task.Activity,
            State = task.State.GetDisplayName(),
            Priority = task.Priority.GetDisplayName(),
            CreatedOn = task.CreatedOn,
            AssignedOn = task.AssignedOn,
            DueDate = task.DueDate
        };

        return new GlobalResponse<TaskDto>(true, "create task success", value: taskToReturn);

    }

    public async Task<GlobalResponse<TaskDto>> GetTaskById(Guid id)
    {
        var task = await _repository.Tasks.Where(t => t.Id == id).Select(x => new TaskDto()
        {
            Id = x.Id,
            Activity = x.Activity,
            State = x.State.GetDisplayName(),
            Priority = x.Priority.GetDisplayName(),
            IsComplete = x.IsComplete,
            AssignedOn = x.AssignedOn,
            DueDate = x.DueDate
            
        }).FirstOrDefaultAsync();

        if (task == null) return new GlobalResponse<TaskDto>(false, "get task failed", errors: [$"task with id: {id} not found "]);

        return new GlobalResponse<TaskDto>(true, "get task success", value: task);
    }

    public async Task<GlobalResponse<TaskDto>> UpdateTask(Guid id, UpdateTaskDto updateCommand)
    {
        var task = await _repository.Tasks.FindAsync(id);

        if (task == null) return new GlobalResponse<TaskDto>(false, "update task failed", errors: [$"task with id: {id} not found"]);

        var updatedTask = _mapper.Map(updateCommand, task);

        var validationResult = new TaskValidator().Validate(updatedTask);

        if (!validationResult.IsValid) return new GlobalResponse<TaskDto>(false, "update task failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.SaveChangesAsync();

        return new GlobalResponse<TaskDto>(true, "update task success");
    }

    public async Task<GlobalResponse<TaskDto>> PatchTask(Guid id, UpdateTaskDto patchCommand)
    {
        var task = await _repository.Tasks.FindAsync(id);

        if (task == null) return new GlobalResponse<TaskDto>(false, "patch task failed", errors: [$"tasks with id: {id} not found"]);

        var taskToBePatched = _mapper.Map<UpdateTaskDto>(task);

        var patchedTaskDto = Patcher.Patch(patchCommand, taskToBePatched);

        var patchedTask = _mapper.Map(patchedTaskDto, task);

        var validationResult = new TaskValidator().Validate(patchedTask);
        if (!validationResult.IsValid) return new GlobalResponse<TaskDto>(false, "patch task failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        if (task.IsComplete)
        {
            task.State = TaskState.Done;
        }

        if (!task.IsComplete && task.MemberId != null)
        {
            task.State = TaskState.InProgress;
        }
        else
        {
            task.State = TaskState.Pending;
        }

        await _repository.SaveChangesAsync();

        return new GlobalResponse<TaskDto>(true, "patch task success");
    }

    public async Task<GlobalResponse<TaskDto>> DeleteTask(Guid id)
    {
        var task = await _repository.Tasks.FindAsync(id);

        if (task == null) return new GlobalResponse<TaskDto>(false, "delete task failed", errors: [$"task with id: {id} not found"]);

        _repository.Tasks.Remove(task);

        await _repository.SaveChangesAsync();

        return new GlobalResponse<TaskDto>(true, "delete task success");
    }
}

