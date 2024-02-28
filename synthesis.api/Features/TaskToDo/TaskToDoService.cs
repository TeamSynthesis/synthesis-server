using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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
        var projectExists = await _repository.Projects.AnyAsync(p => p.Id == projectId);

        if (!projectExists) return new GlobalResponse<TaskDto>(false, "create task failed", errors: [$"project with id: {projectId} not found"]);

        var task = new TaskToDoModel()
        {
            ProjectId = projectId,
            Activity = createCommand.Activity,
            State = TaskState.Pending,
            Priority = createCommand.Priority,
            CreatedOn = DateTime.UtcNow
        };

        var validationResult = new TaskValidator().Validate(task);

        if (!validationResult.IsValid) return new GlobalResponse<TaskDto>(false, "create task failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.Tasks.AddAsync(task);

        await _repository.SaveChangesAsync();

        var taskToReturn = new TaskDto()
        {
            Id = task.Id,
            Activity = task.Activity,
            State = task.State,
            Priority = task.Priority,
            CreatedOn = task.CreatedOn
        };

        return new GlobalResponse<TaskDto>(true, "create task success", value: taskToReturn);

    }

    public async Task<GlobalResponse<TaskDto>> GetTaskById(Guid id)
    {
        var task = await _repository.Tasks.Where(t => t.Id == id).Select(x => new TaskDto()
        {
            Id = x.Id,
            Activity = x.Activity,
            State = x.State,
            Priority = x.Priority
        }).SingleOrDefaultAsync();

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

