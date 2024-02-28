using FluentValidation;

namespace synthesis.api.Features.Auth;

public class AuthValidator : AbstractValidator<RegisterUserDto>
{
    private readonly string pattern
   = @"^[a-zA-Z0-9][a-zA-Z0-9_.-]*[a-zA-Z0-9]$";
    public AuthValidator()
    {

        RuleFor(u => u.Password)
        .NotNull().NotEmpty().WithMessage("Password must not be empty")
        .MinimumLength(6).WithMessage("Password must be at least 6 characters")
        .MaximumLength(60).WithMessage("Password must not exceed 60 characters")
        .Matches(@"^(?=.*[A-Z])(?=.*[^a-zA-Z0-9\s]).+$").WithMessage("Password must have at least one uppercase and a special character excluding spaces");


        RuleFor(u => u.Email)
        .NotNull().NotEmpty().WithMessage("Email must not be empty")
        .EmailAddress().WithMessage("Email must be a valid email addresss");
    }
}