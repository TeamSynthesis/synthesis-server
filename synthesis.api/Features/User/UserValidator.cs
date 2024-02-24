using FluentValidation;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;

namespace synthesis.api.Features.User;

public class UserValidator : AbstractValidator<UserModel>
{
    private readonly string pattern
    = @"^[a-zA-Z0-9][a-zA-Z0-9_.-]*[a-zA-Z0-9]$";

    public UserValidator(RepositoryContext _repository, UserModel? userToBeUpdated = null)
    {

        RuleFor(u => u.UserName)
        .NotNull().NotEmpty().WithMessage("Username must not be empty")
        .Length(2, 20).WithMessage("Username must be between 2 - 20 characters")
        .Matches(pattern).WithMessage("Username must start and end with alphanumeric characters, with optional special characters ( _.- )")
        .MustAsync(async (username, _) =>
            {
                if (userToBeUpdated != null)
                {

                    if (userToBeUpdated.UserName.ToLower() == username.ToLower()) return true;

                    return !await _repository.Users.AnyAsync(u => u.UserName == username);
                }

                return !await _repository.Users.AnyAsync(u => u.UserName == username);
            }
        ).WithMessage("username must be unique");

        RuleFor(u => u.Email)
        .NotNull().NotEmpty().WithMessage("Email must not be empty")
        .EmailAddress().WithMessage("Email must be a valid email addresss")
        .MustAsync(async (email, _) =>
            {
                if (userToBeUpdated != null)
                {
                    if (userToBeUpdated.Email.ToLower() == email.ToLower()) return true;
                    return !await _repository.Users.AnyAsync(u => u.Email == email);
                }

                return !await _repository.Users.AnyAsync(u => u.Email == email);
            }
        ).WithMessage("email must be unique");
    }

}
