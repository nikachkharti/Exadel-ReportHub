using FluentValidation;
using ReportHub.Application.Features.Clients.Commands;

namespace ReportHub.Application.Validators.ClientValidators.ClientCommandValidators;

public class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
{
    public DeleteClientCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}
