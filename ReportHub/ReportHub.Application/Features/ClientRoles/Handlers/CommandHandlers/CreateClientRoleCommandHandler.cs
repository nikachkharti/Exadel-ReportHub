using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.ClientRoles.Commands;
using ReportHub.Application.Validators.Exceptions;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.ClientRoles.Handlers.CommandHandlers;

public class CreateClientRoleCommandHandler : IRequestHandler<CreateClientRoleCommand, ClientRole>
{
    private readonly IClientRoleRepository _clientRoleRepository;

    public CreateClientRoleCommandHandler(IClientRoleRepository clientRoleRepository)
    {
        _clientRoleRepository = clientRoleRepository;
    }

    public async Task<ClientRole> Handle(CreateClientRoleCommand request, CancellationToken cancellationToken)
    {
        await EnsureRoleDoesNotExist(request.RoleName, cancellationToken);

        var clientRole = new ClientRole { Name = request.RoleName };

        await _clientRoleRepository.Insert(clientRole, cancellationToken);

        return clientRole;
    }

    private async Task EnsureRoleDoesNotExist(string role, CancellationToken cancellationToken)
    {
        var existingRole = await _clientRoleRepository.Get(r => r.Name.Equals(role), cancellationToken);

        if(existingRole is not null)
        {
            throw new BadRequestException($"Client role with name {role} already exist");
        }
    }
}
