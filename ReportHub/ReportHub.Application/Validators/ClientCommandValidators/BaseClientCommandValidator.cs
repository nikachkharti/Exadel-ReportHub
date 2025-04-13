using FluentValidation;
using ReportHub.Application.Features.Clients.Commands;

namespace ReportHub.Application.Validators.ClientCommandValidators;

public class BaseClientCommandValidator<TCommand> : AbstractValidator<TCommand> 
    where TCommand : BaseClientCommand
{
    public BaseClientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required!");

        RuleFor(x => x.Specialization)
            .NotEmpty()
            .WithMessage("Specialization is required!");
    }
}
