using synthesis.api.Data.Models;
using synthesis.api.Features.Feature;
using synthesis.api.Services.OpenAi.Dtos;

namespace synthesis.api.Features.Project;

public record ProjectDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedOn { get; set; }
    public TeamDto? Team { get; set; }
    public List<FeatureDto> Features { get; set; }
    public List<TaskDto>? Tasks { get; set; }
    public PrePlanMetaData? PrePlan { get; set; }
}

public record GeneratedProjectDto
{
    public PrePlanMetaData? Metadata { get; set; }
}

public record CreateProjectDto(string? Name, string? Description);

public record UpdateProjectDto(string Name, string Description);

public record PlanDto
{
    public Guid Id { get; set; }
    
    public Guid TeamId { get; set; }
    public GeneratedPrePlanDto? Plan { get; set; }

    public PlanStatus Status { get; set; }

    public bool IsSuccess { get; set; }
}
