namespace synthesis.api.Features.User;

public record UserDto(Guid Id, string FirstName, string LastName, string Username, string AvatarUrl, string Email, string Password); 

public record UpdateUserDto(string FirstName, string LastName, string Username, string AvatarUrl, string Email, string Password);
