using synthesis.api.Features.User;

namespace synthesis.api.Features.Auth;

public record RegisterUserDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public record GitHubUserDto
{
    public string? id { get; set; }
    public string? login { get; set; }
    public string? avatar_url { get; set; }
    public string? name { get; set; }
}

public record GitHubEmailDto
{
    public string? email { get; set; }
    public bool? primary { get; set; }
    public bool? verified { get; set; }
}

public record RegisterResponseDto(string token, UserDto User);
public record LoginResponseDto(string token);
public record LoginUserDto(string Email, string Password);