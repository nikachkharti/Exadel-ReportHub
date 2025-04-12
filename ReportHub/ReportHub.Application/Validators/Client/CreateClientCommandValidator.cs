using FluentValidation;
using ReportHub.Application.Features.Clients.Commands;

namespace ReportHub.Application.Validators.Client;
public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(dto => dto.Name)
            .Length(3, 100)
            .WithMessage("Please use a name length in range of [3 - 100] !");
    }
}
