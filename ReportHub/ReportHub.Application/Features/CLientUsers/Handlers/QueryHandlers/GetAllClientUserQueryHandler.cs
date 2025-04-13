using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.CLientUsers.Queries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.CLientUsers.Handlers.QueryHandlers;

public class GetAllClientUserQueryHandler : IRequestHandler<GetAllClientUserQuery, IEnumerable<ClientUser>>
{
    private readonly IClientUserRepository _clientUserRepository;
    public GetAllClientUserQueryHandler(IClientUserRepository clientUserRepository)
    {
        _clientUserRepository = clientUserRepository;
    }
    public async Task<IEnumerable<ClientUser>> Handle(GetAllClientUserQuery request, CancellationToken cancellationToken)
    {
        return await _clientUserRepository.GetAll(cancellationToken);
    }
}
{
}
