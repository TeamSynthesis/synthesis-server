
using synthesis.api.Data.Models;

public record TaskDto
{
    public Guid Id { get; set; }
    public string? Activity { get; set; }
    public string? State { get; set; }
    public string? Priority { get; set; }
    public bool IsComplete { get; set; }
    public string? CreatedOn { get; set; }
    public string? AssignedOn { get; set; }
    public string? DueDate { get; set; }
}

public record CreateTaskDto(string Activity, TaskPriority Priority = TaskPriority.Normal);
public record UpdateTaskDto
{
    public string? Activity { get; set; }
    public TaskPriority? Priority { get; set; }
    public bool? IsComplete { get; set; }
}

