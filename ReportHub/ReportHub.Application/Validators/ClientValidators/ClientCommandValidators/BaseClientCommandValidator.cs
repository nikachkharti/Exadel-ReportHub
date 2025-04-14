using FluentValidation;
using ReportHub.Application.Features.Clients.Commands;

namespace ReportHub.Application.Validators.ClientValidators.ClientCommandValidators;

public class BaseClientCommandValidator<TCommand> : AbstractValidator<TCommand> 
    where TCommand : BaseClientCommand
{
    public BaseClientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Specialization)
            .NotEmpty().WithMessage("Specialization is required.")
            .MaximumLength(120).WithMessage("Specialization must not exceed 120 characters.");
    }
}
