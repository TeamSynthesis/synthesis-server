using FluentValidation;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;

namespace synthesis.api.Features.User;

public class UserValidator : AbstractValidator<UserModel>
{
    private readonly string pattern
    = @"^[a-zA-Z0-9][a-zA-Z0-9_.-]*[a-zA-Z0-9]$";

    public UserValidator()
    {

        RuleFor(u => u.UserName)
        .NotNull().NotEmpty().WithMessage("Username must not be empty")
        .Length(2, 20).WithMessage("Username must be between 2 - 20 characters")
        .Matches(pattern).WithMessage("Username must start and end with alphanumeric characters, with optional special characters ( _.- )");

        RuleFor(u => u.Email)
        .NotNull().NotEmpty().WithMessage("Email must not be empty")
        .EmailAddress().WithMessage("Email must be a valid email addresss");
    }

}
