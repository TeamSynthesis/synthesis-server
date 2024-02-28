
using synthesis.api.Data.Models;

public record TaskDto
{
    public Guid Id { get; set; }
    public string? Activity { get; set; }
    public TaskState State { get; set; }
    public TaskPriority Priority { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? AssignedOn { get; set; }
    public DateTime? DueDate { get; set; }
}

public record CreateTaskDto(string Activity, TaskPriority Priority = TaskPriority.Normal);
public record UpdateTaskDto
{
    public string? Activity { get; set; }
    public TaskPriority? Priority { get; set; }
    public bool? IsComplete { get; set; }
}

