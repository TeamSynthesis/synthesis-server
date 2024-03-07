
using synthesis.api.Features.Project;

public record TeamDto
{
    public Guid Id { get; set; }
    public string? Slug { get; set; }
    public string? Name { get; set; }
    public string? AvatarUrl { get; set; }
    public List<MemberDto>? Members { get; set; }
    public List<ProjectDto>? Projects { get; set; }
}

public record CreateTeamDto(string Name, string Slug, IFormFile? Avatar);
public record UpdateTeamDto(string Name, string LogoUrl);