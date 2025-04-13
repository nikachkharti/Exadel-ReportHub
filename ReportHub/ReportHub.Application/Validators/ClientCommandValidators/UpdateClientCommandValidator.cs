using FluentValidation;
using ReportHub.Application.Features.Clients.Commands;

namespace ReportHub.Application.Validators.ClientCommandValidators;

public class UpdateClientCommandValidator : BaseClientCommandValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}
