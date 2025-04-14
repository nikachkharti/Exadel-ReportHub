using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.CLientUsers.Queries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.CLientUsers.Handlers.QueryHandlers;

public class GetAllClientUserByClientIdQueryHandler : IRequestHandler<GetAllClientUserByClientIdQuery, IEnumerable<ClientUser>>
{
    private readonly IClientUserRepository _clientUserRepository;
    public GetAllClientUserByClientIdQueryHandler(IClientUserRepository clientUserRepository)
    {
        _clientUserRepository = clientUserRepository;
    }
    public Task<IEnumerable<ClientUser>> Handle(GetAllClientUserByClientIdQuery request, CancellationToken cancellationToken)
    {
        return _clientUserRepository.GetAll(c => c.ClientId == request.ClientId, cancellationToken);
    }
}
