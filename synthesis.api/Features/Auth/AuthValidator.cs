using FluentValidation;

namespace synthesis.api.Features.Auth;

public class AuthValidator : AbstractValidator<RegisterUserDto>
{
    private readonly string pattern
   = @"^[a-zA-Z0-9][a-zA-Z0-9_.-]*[a-zA-Z0-9]$";
    public AuthValidator()
    {

        RuleFor(u => u.Username)
        .NotNull().NotEmpty().WithMessage("Username must not be empty")
        .Length(2, 20).WithMessage("Username must be between 2 - 20 characters")
        .Matches(pattern).WithMessage("Username must start and end with alphanumeric characters, with optional special characters ( _.- )");

        RuleFor(u => u.Email)
        .NotNull().NotEmpty().WithMessage("Email must not be empty")
        .EmailAddress().WithMessage("Email must be a valid email addresss");
    }
}