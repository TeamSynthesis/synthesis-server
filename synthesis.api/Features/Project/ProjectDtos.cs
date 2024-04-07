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
    public List<TaskDto>? Tasks { get; set; }
    public ProjectMetadata? Metadata { get; set; }
}

public record GeneratedProjectDto
{
    public ProjectMetadata? Metadata { get; set; }
}

public record CreateProjectDto(string? Name, string? Description);

public record UpdateProjectDto(string Name, string Description);

public record PlanDto
{
    public Guid Id { get; set; }

    public GptProjectDto? Plan { get; set; }

    public bool IsSuccess { get; set; }
}