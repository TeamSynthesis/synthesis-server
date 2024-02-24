using synthesis.api.Features.User;

namespace synthesis.api.Features.Auth;

public record RegisterUserDto
{
    public string? Username { get; set; }
    public IFormFile? AvatarUrl { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}
public record RegisterResponseDto(string token, UserDto User);
public record LoginResponseDto(string token);
public record LoginUserDto(string Email, string Password);