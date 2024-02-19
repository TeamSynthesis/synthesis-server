using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class TaskToDoModel
{
    [Column("TaskId")]
    public Guid Id { get; set; }
    public string? Activity { get; set; }
    public TaskState State { get; set; } 
    public TaskPriority Priority { get; set; }

    public bool IsComplete { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? AssignedOn { get; set; }
    public DateTime? DueDate { get; set; }


    [ForeignKey(nameof(ProjectModel))]
    public Guid ProjectId { get; set; }
    public ProjectModel? Project { get; set; }

    [ForeignKey(nameof(FeatureModel))]
    public Guid? FeatureId { get; set; }
    public FeatureModel? Feature { get; set; }

    [ForeignKey(nameof(MemberModel))]
    public Guid? MemberId { get; set; }
    public MemberModel? Member { get; set; }
}

public enum TaskState
{
    Pending,
    InProgress,
    Done
}

public enum TaskPriority
{
    Low,
    Normal,
    High
}