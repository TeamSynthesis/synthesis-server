
using synthesis.api.Features.User;

public record MemberDto(Guid Id, UserDto User, Guid OrganisationId, List<string> Roles);
public record MemberProfileDto(Guid Id, Guid OrganisationId, List<string> Roles);

