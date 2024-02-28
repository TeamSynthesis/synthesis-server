using synthesis.api.Data.Models;

namespace synthesis.api.Features.User;

public record UserDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? Profession { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Email { get; set; }
    public string? OnBoarding { get; set; }
    public List<string>? Skills { get; set; }
    public List<MemberDto>? MemberProfiles { get; set; }

}

public record PostUserDetailsDto
{
    public IFormFile? Avatar { get; set; }
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public string? Profession { get; set; }

}


public record UpdateUserDto(string FirstName, string LastName, string Username, IFormFile Avatar, string Email);
