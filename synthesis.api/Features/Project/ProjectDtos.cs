using synthesis.api.Data.Models;

namespace synthesis.api.Features.Project;

public record ProjectDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? IconUrl { get; set; }
    public ProjectMetadata? Metadata { get; set; }
}

public record GeneratedProjectDto
{
    public ProjectMetadata? Metadata { get; set; }
}

public record CreateProjectDto(string Name, string Description);
public record UpdateProjectDto(string Name, string Description);

