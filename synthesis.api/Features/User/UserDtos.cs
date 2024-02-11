namespace synthesis.api.Features.User;

public record UserDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Email { get; set; }
    public List<MemberDto>? MemberProfiles { get; set; }

}

public record RegisterUserDto(string FirstName, string LastName, string Username, IFormFile Avatar, string Email, string Password);
public record UpdateUserDto(string FirstName, string LastName, string Username, IFormFile Avatar, string Email);
