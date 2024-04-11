
using Octokit;
using synthesis.api.Features.Project;

public record TeamDto
{
    public Guid Id { get; set; }
    public string? Slug { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? AvatarUrl { get; set; }
    public List<MemberDto>? Members { get; set; }
    public List<ProjectDto>? Projects { get; set; }
}

public record CreateTeamDto(string Name, string Description, string Slug, string? AvatarUrl);
public record UpdateTeamDto(string Name, string Description);

public record MemberInviteDto
{
    public string? Email { get; set; }
    public string? Role { get; set; }
}
public record InviteDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public DateTime InvitedOn { get; set; }
    public string? Email { get; set; }

    public string? Role { get; set; }
    public bool Accepted { get; set; }
}

public record CreateInviteDto
{
    public string? Code { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
}