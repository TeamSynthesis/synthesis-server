
using synthesis.api.Features.Project;

public record OrganisationDto(Guid Id, string Name, string LogoUrl, List<MemberDto> Members, List<ProjectDto> Projects);
public record CreateOrganisationDto(string Name, string LogoUrl);
public record UpdateOrganisationDto(string Name, string LogoUrl);