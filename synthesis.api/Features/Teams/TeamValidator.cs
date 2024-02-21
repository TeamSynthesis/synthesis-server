using FluentValidation;
using synthesis.api.Data.Models;

namespace synthesis.api.Features.Teams;

public class TeamValidator : AbstractValidator<TeamModel>
{
    private readonly string pattern = @"^[a-zA-Z][a-zA-Z_.-]*[a-zA-Z]$";
    public TeamValidator()
    {
        RuleFor(org => org.Name)
        .NotNull().NotEmpty().WithMessage("name cannot be null")
        .MaximumLength(50).WithMessage("name cannot exceed 50 characters")
        .Matches(pattern).WithMessage("name must start and end with letters, with optional special characters ( _.- ) inbetween");

        RuleFor(org => org.LogoUrl)
        .Matches("[a-zA-z]")
        .When(org => !string.IsNullOrEmpty(org.LogoUrl));
    }
}