using FluentValidation;
using ReportHub.Application.Features.Item.Commands;

namespace ReportHub.Application.Validators.ItemValidators.ItemCommandValidators;

public class DeleteItemOfClientCommandValidator : AbstractValidator<DeleteItemOfClientCommand>
{
    public DeleteItemOfClientCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client ID is required.");

        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("Item ID is required.");
    }
}
