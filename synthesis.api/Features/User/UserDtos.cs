using synthesis.api.Data.Models;

namespace synthesis.api.Features.User;

public record UserDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? Profession { get; set; }
    public string? AvatarUrl { get; set; }
    public bool? EmailConfirmed { get; set; }
    public string? Email { get; set; }
    public string? OnBoarding { get; set; }
    public DateTime? CreatedOn { get; set; }
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

public record UpdateUserDto(string FullName, string Username, string Email, string Profession);
