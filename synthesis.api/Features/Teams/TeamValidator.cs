using FluentValidation;
using synthesis.api.Data.Models;

namespace synthesis.api.Features.Teams;

public class TeamValidator : AbstractValidator<TeamModel>
{

    private readonly string pattern = @"^[a-zA-Z][a-zA-Z_-]*[a-zA-Z]$";
    public TeamValidator()
    {
        RuleFor(t => t.Name)
        .NotNull().NotEmpty().WithMessage("name is a required field")
        .MaximumLength(64).WithMessage("name cannot exceed 64 characters");


        RuleFor(t => t.Slug)
        .NotNull().NotEmpty().WithMessage("slug is a required field")
        .Length(3, 64).WithMessage("slug must be between 3-64 chars")
        .Matches(pattern).WithMessage("name must start and end with letters, with optional special characters ( _- ) inbetween");


    }
}