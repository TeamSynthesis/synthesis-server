using FluentValidation;
using Microsoft.VisualBasic;
using synthesis.api.Data.Models;

namespace synthesis.api.Features.Project;

public class ProjectValidator : AbstractValidator<ProjectModel>
{
    private readonly string pattern = @"^[a-zA-Z][a-zA-Z_.-]*[a-zA-Z]$";
    public ProjectValidator()
    {
        // RuleFor(p => p.Name)
        // .NotNull().WithMessage("name cannot be null")
        // .Matches(pattern).WithMessage("name must start and end with letters, with optional special characters ( _.- ) inbetween");

        // RuleFor(p => p.Description)
        // .NotNull().NotEmpty().WithMessage("name cannot be null");
    }
}