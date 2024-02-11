
using synthesis.api.Features.Team;
using synthesis.api.Features.User;

public record MemberDto()
{
    public Guid Id { get; set; }
    public UserDto? User { get; set; }
    public OrganisationDto? Organisation { get; set; }
    public List<string>? Roles { get; set; }
    public List<TeamDto>? Teams { get; set; }
}
