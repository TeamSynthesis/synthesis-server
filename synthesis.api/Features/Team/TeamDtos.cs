namespace synthesis.api.Features.Team;

public record TeamDto()
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<MemberDto>? Members { get; set; }
}

public record CreateTeamDto(string Name, string Description);
public record UpdateTeamDto(string Name, string Description);