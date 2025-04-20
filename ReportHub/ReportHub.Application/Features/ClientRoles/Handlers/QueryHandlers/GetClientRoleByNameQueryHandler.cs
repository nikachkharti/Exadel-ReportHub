using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.ClientRoles.Queries;
using ReportHub.Application.Validators.Exceptions;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.ClientRoles.Handlers.QueryHandlers;

public class GetClientRoleByNameQueryHandler : IRequestHandler<GetClientRoleByNameQuery, ClientRole>
{
    private readonly IClientRoleRepository _clientRoleRepository;

    public GetClientRoleByNameQueryHandler(IClientRoleRepository clientRoleRepository)
    {
        _clientRoleRepository = clientRoleRepository;
    }

    public async Task<ClientRole> Handle(GetClientRoleByNameQuery request, CancellationToken cancellationToken)
    {
        var clientRole = await _clientRoleRepository.Get(c => c.Name.Equals(request.RoleName), cancellationToken);

        if (clientRole is null)
        {
            throw new NotFoundException($"Client role with name {request.RoleName} not found");
        }

        return clientRole;
    }
}
