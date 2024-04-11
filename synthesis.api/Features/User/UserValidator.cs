using FluentValidation;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;

namespace synthesis.api.Features.User;

public class UserValidator : AbstractValidator<UserModel>
{
    private readonly string pattern
    = @"^[a-zA-Z0-9][a-zA-Z0-9_.-]*[a-zA-Z0-9]$";

    public UserValidator()
    {
        RuleFor(u => u.UserName)
        .NotNull().NotEmpty().WithMessage("Username is a required field")
        .Length(2, 20).WithMessage("Username must be between 2 - 20 characters")
        .Matches(pattern).WithMessage("Username must start and end with alphanumeric characters, with optional special characters ( _.- )");

        RuleFor(u => u.Profession)
        .NotNull().NotEmpty().WithMessage("Profession is a required field")
        .Length(2, 20).WithMessage("Profession must be between 2 - 20 characters");

        RuleFor(u => u.FullName)
        .Matches("^[a-zA-Z ]*$").WithMessage("Full name may contain only letters and spaces")
        .When(u => !string.IsNullOrEmpty(u.FullName));
    }

}
