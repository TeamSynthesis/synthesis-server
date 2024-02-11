namespace synthesis.api.Features.Project;

public record ProjectDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
public record CreateProjectDto(string Name, string Description);
public record UpdateProjectDto(string Name, string Description);

