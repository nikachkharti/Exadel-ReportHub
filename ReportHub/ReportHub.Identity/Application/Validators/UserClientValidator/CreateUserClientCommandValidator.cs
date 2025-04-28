using FluentValidation;
using MongoDB.Bson;
using ReportHub.Identity.Application.Features.UserClients.Commands;

namespace ReportHub.Identity.Application.Validators.UserClientValidator;

public class CreateUserClientCommandValidator : AbstractValidator<CreateUserClientCommand>
{
    public CreateUserClientCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull()
            .WithMessage("User is required")
            .Must(userId => ObjectId.TryParse(userId, out var id))
            .WithMessage("User id is not valid");


        RuleFor(x => x.ClientId)
            .NotNull()
            .WithMessage("User is required")
            .Must(clientId => ObjectId.TryParse(clientId, out var id))
            .WithMessage("User id is not valid");

        RuleFor(x => x.Role)
            .NotNull()
            .NotEmpty()
            .WithMessage("Role is required");
    }
}
