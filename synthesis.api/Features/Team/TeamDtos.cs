namespace synthesis.api.Features.Team;

public record TeamDto(Guid id, string Name, string Description, List<MemberDto> Members);
public record TeamProfileDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
public record CreateTeamDto(string Name, string Description);
public record UpdateTeamDto(string Name, string Description);