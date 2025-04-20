using FluentValidation;
using ReportHub.Application.Features.ClientRoles.Commands;

namespace ReportHub.Application.Validators.ClientRoleValidator.ClientRoleCommandValidators;

public class CreateClientRoleCommandValidator : AbstractValidator<CreateClientRoleCommand>
{
    public CreateClientRoleCommandValidator()
    {
        RuleFor(r => r.RoleName)
            .NotEmpty()
            .WithMessage("Role name is required.")
            .Length(1, 50)
            .WithMessage("Role name must be between 1 and 50 characters long.")
            .Matches(@"^[a-zA-Z]+$")
            .WithMessage("Role name can only contain alphanumeric characters and spaces.");
    }
}
