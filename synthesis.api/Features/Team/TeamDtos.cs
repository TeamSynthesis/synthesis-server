namespace synthesis.api.Features.Team;

public record TeamDto(Guid id, string Name, string Description);
public record CreateTeamDto(string Name, string Description);
public record UpdateTeamDto(string Name, string Description);