
using synthesis.api.Data.Models;

public record TaskDto
{
    public Guid Id { get; set; }
    public string? Activity { get; set; }
    public string? State { get; set; }
    public string? Priority { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? AssignedOn { get; set; }
    public DateTime? DueDate { get; set; }
}

public record CreateTaskDto
{
    public string Activity { get; set; }
    public TaskPriority Priority { get; set; }


    public DateTime DueDate { get; set; }
    
}

public record UpdateTaskDto
{
    public string? Activity { get; set; }
    public TaskPriority? Priority { get; set; }
    public bool? IsComplete { get; set; }
}

