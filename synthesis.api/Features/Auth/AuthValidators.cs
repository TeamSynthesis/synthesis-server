using FluentValidation;

namespace synthesis.api.Features.Auth;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{

    private readonly string pattern
    = @"^[a-zA-Z0-9][a-zA-Z0-9_.-]*[a-zA-Z0-9]$";

    public RegisterUserDtoValidator()
    {

        RuleFor(u => u.FirstName)
        .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
        .Matches("[a-zA-Z]+")
        .When(u => !string.IsNullOrEmpty(u.FirstName)).WithMessage("First name can only contain letters");

        RuleFor(u => u.LastName)
        .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
        .Matches("[a-zA-Z]+")
        .When(u => !string.IsNullOrEmpty(u.LastName)).WithMessage("Last name can only contain letters"); // 

        RuleFor(u => u.UserName)
        .NotEmpty().WithMessage("Username cannot be empty")
        .Length(2, 20).WithMessage("Username must be between 2 - 20 characters")
        .Matches(pattern).WithMessage("Username must start and end with alphanumeric characters, with optional special characters ( _.- )");

        RuleFor(u => u.AvatarUrl)
        .Matches("[www].[A-Za-z].[com]");

        RuleFor(u => u.Email)
        .NotEmpty().WithMessage("Email cannot be empty")
        .EmailAddress().WithMessage("Email must be a valid email addresss");

        RuleFor(u => u.Password)
        .NotEmpty().WithMessage("Password cannot be empty")
        .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
        .Matches("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}")
        .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");

    }
}
