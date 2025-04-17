using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.ClientRoles.Queries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.ClientRoles.Handlers.QueryHandlers;

public class GetAllClientRolesQueryHandler : IRequestHandler<GetAllClientRolesQuery, IEnumerable<ClientRole>>
{
    private readonly IClientRoleRepository _clientRoleRepository;

    public GetAllClientRolesQueryHandler(IClientRoleRepository clientRoleRepository)
    {
        _clientRoleRepository = clientRoleRepository;
    }

    public Task<IEnumerable<ClientRole>> Handle(GetAllClientRolesQuery request, CancellationToken cancellationToken)
    {
        return _clientRoleRepository.GetAll(cancellationToken);
    }
}
