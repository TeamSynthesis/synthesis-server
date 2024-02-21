using System.Data;
using FluentValidation;
using synthesis.api.Data.Models;

namespace synthesis.api.Features.Feature;

public class FeatureValidator : AbstractValidator<FeatureModel>
{

    public FeatureValidator()
    {
        RuleFor(ft => ft.Name)
        .NotNull().NotEmpty().WithMessage("name is a required field");

        RuleFor(ft => ft.Type)
        .IsInEnum().WithMessage("invalid feature type");
    }
}