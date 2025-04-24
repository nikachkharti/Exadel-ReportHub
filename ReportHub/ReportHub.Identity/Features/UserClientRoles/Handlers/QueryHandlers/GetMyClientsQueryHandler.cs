using MediatR;
using ReportHub.Identity.Features.UserClientRoles.DTOs;
using ReportHub.Identity.Features.UserClientRoles.Queries;

namespace ReportHub.Identity.Features.UserClientRoles.Handlers.QueryHandlers;

public class GetMyClientsQueryHandler : IRequestHandler<GetMyClientsQuery, IEnumerable<MyClientForGettingDto>>
{
    public Task<IEnumerable<MyClientForGettingDto>> Handle(GetMyClientsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
