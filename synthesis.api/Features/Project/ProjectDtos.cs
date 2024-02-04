namespace synthesis.api.Features.Project;

public record ProjectDto(Guid Id, string Name, string Description);
public record CreateProjectDto(string Name, string Description);

public record UpdateProjectDto(string Name, string Description);

