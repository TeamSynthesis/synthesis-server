
using synthesis.api.Features.Team;
using synthesis.api.Features.User;

public record MemberDto(Guid Id, UserDto User, OrganisationDto Organisation, List<string> Roles, List<TeamDto> Teams);

public record MemberProfileDto
{
    public Guid Id { get; set; }
    public OrganisationProfileDto? Organisation { get; set; }
    public List<string>? Roles { get; set; }
}

public record TeamMemberDto
{
    public Guid Id { get; set; }
    public UserDto? User { get; set; }

}
public record OrganisationMemberDto
{
    public Guid Id { get; set; }
    public UserDto? User { get; set; }
    public List<string>? Roles { get; set; }
}