namespace synthesis.api.Features.User;

public record UserDto(Guid Id, string FirstName, string LastName, string Username, string AvatarUrl, string Email);
public record UserProfileDto(Guid Id, string FirstName, string LastName, string Username, string AvatarUrl, string Email, List<MemberProfileDto> MemberProfiles);
public record RegisterUserDto(string FirstName, string LastName, string Username, IFormFile Avatar, string Email, string Password);
public record UpdateUserDto(string FirstName, string LastName, string Username, string AvatarUrl, string Email);
