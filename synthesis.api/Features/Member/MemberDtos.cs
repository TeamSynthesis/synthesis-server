
using synthesis.api.Features.Team;
using synthesis.api.Features.User;

public record MemberDto(Guid Id, UserDto User, Guid OrganisationId, List<string> Roles, List<TeamDto> Teams);
public record MemberProfileDto(Guid Id, Guid OrganisationId, List<string> Roles);

