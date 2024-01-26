using FluentValidation;
using synthesis.api.Data.Models;

namespace synthesis.api.Features.User;

public class UserValidator : AbstractValidator<UserModel>
{
    private readonly string pattern
    = @"^[a-zA-Z0-9][a-zA-Z0-9_.-]*[a-zA-Z0-9]$";

    public UserValidator()
    {

        RuleFor(u => u.FirstName)
        .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
        .Matches("[a-zA-Z]+").WithMessage("First name can only contain letters")
        .When(u => !string.IsNullOrEmpty(u.FirstName));

        RuleFor(u => u.LastName)
        .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
        .Matches("[a-zA-Z]+").WithMessage("LastName may contain letter only")
        .When(u => !string.IsNullOrEmpty(u.LastName));

        RuleFor(u => u.UserName)
        .NotNull().WithMessage("Username must not be empty")
        .Length(2, 20).WithMessage("Username must be between 2 - 20 characters")
        .Matches(pattern).WithMessage("Username must start and end with alphanumeric characters, with optional special characters ( _.- )");

        RuleFor(u => u.AvatarUrl)
        .Matches("[A-Za-z]")
        .When(u => !string.IsNullOrEmpty(u.AvatarUrl));


        RuleFor(u => u.Email)
        .NotNull().WithMessage("Email must not be empty")
        .EmailAddress().WithMessage("Email must be a valid email addresss");

        RuleFor(u => u.Password)
        .NotNull().WithMessage("Password must not be empty")
        .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
        .Matches("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}")
        .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");
    }
}