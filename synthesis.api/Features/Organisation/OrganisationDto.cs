
public record OrganisationDto(Guid Id, string Name, string LogoUrl);
public record CreateOrganisationDto(string Name, string LogoUrl);

public record UpdateOrganisationDto(string Name, string LogoUrl);