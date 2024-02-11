
using synthesis.api.Features.Project;

public record OrganisationDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public List<MemberDto>? Members { get; set; }
    public List<ProjectDto>? Projects { get; set; }
}

public record CreateOrganisationDto(string Name, string LogoUrl);
public record UpdateOrganisationDto(string Name, string LogoUrl);