
using synthesis.api.Features.User;

public record MemberDto()
{
    public Guid Id { get; set; }
    public UserDto? User { get; set; }
    public TeamDto? Team { get; set; }
    public List<string>? Roles { get; set; }
}
