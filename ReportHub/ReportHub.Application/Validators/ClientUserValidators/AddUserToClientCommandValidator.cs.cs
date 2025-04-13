using FluentValidation;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.CLientUsers.Commands;

namespace ReportHub.Application.Validators.ClientUserValidators;

public class AddUserToClientCommandValidator : AbstractValidator<AddUserToClientCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IClientRepository _clientRepository;
    private readonly IClientUserRepository _clientUserRepository;

    public AddUserToClientCommandValidator(
        IIdentityService identityService, IClientRepository clientRepository, IClientUserRepository clientUserRepository)
    {
        _identityService = identityService;
        _clientRepository = clientRepository;
        _clientUserRepository = clientUserRepository;

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required.")
            .MustAsync(ValidateClientExist)
            .WithMessage("Client does not exist"); ;


        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.")
            .MustAsync(_identityService.ValidateUserIdExists)
            .WithMessage("User does not exist or you have no access to assign role");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required.")
            .MustAsync(_identityService.ValidateRoleExists)
            .WithMessage("Role does not exist or you do not have access to give role");

        RuleFor(x => x)
            .MustAsync(EnsureUserRoleNotAssigned)
            .WithMessage("This user has already role for this client");
    }

    private async Task<bool> EnsureUserRoleNotAssigned(AddUserToClientCommand a, CancellationToken cancellationToken)
    {
        return await _clientUserRepository.Get(c => c.UserId == a.UserId && c.ClientId == a.ClientId, cancellationToken) is null;
    }

    private async Task<bool> ValidateClientExist(string clientId, CancellationToken cancellationToken)
    {
        return await _clientRepository.Get(c => c.Id == clientId, cancellationToken) is not null;
    }
}
