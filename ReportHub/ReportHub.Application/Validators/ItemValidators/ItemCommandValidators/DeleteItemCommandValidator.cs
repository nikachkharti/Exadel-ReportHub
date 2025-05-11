using FluentValidation;
using MongoDB.Bson;
using ReportHub.Application.Features.Items.Commands;

namespace ReportHub.Application.Validators.ItemValidators.ItemCommandValidators;

public class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemCommandValidator()
    {
        RuleFor(i => i.Id)
            .NotNull()
            .NotEmpty()
            .WithMessage("Id is required")
            .Must(id => ObjectId.TryParse(id, out var objectId))
            .WithMessage("Id is not valid");
    }
}
