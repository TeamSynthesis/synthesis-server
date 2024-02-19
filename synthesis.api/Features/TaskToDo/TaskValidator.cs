using FluentValidation;
using synthesis.api.Data.Models;

namespace synthesis.api.Features.TaskToDo;

public class TaskValidator : AbstractValidator<TaskToDoModel>
{
    public TaskValidator()
    {
        RuleFor(t => t.Activity)
        .NotEmpty().NotNull().WithMessage("activity cannot be empty");

        RuleFor(t => t.Priority)
        .IsInEnum().WithMessage("priority is invalid");

        RuleFor(t => t.State)
        .IsInEnum().WithMessage("state is invalid");


    }

}