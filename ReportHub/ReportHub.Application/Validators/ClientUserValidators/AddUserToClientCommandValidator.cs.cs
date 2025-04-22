using FluentValidation;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.CLientUsers.Commands;

namespace ReportHub.Application.Validators.ClientUserValidators;

public class AddUserToClientCommandValidator : AbstractValidator<AddUserToClientCommand>
{
    private readonly IClientUserRepository _clientUserRepository;

    public AddUserToClientCommandValidator(IClientUserRepository clientUserRepository)
    {
        _clientUserRepository = clientUserRepository;

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required.");


        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required.");

        RuleFor(x => x)
            .MustAsync(EnsureUserRoleNotAssigned)
            .WithMessage("This user has already role for this client");
    }

    private async Task<bool> EnsureUserRoleNotAssigned(AddUserToClientCommand a, CancellationToken cancellationToken)
    {
        return await _clientUserRepository.Get(c => c.UserId == a.UserId && c.ClientId == a.ClientId, cancellationToken) is null;
    }
}
