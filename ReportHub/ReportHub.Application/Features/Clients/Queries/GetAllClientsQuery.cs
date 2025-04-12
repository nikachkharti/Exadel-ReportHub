using MediatR;
using ReportHub.Application.Features.Clients.DTOs;

namespace ReportHub.Application.Features.Clients.Queries
{
    public record GetAllClientsQuery
    (
        int? PageNumber,
        int? PageSize,
        CancellationToken CancellationToken,
        string SortingParameter = "",
        bool Ascending = false
    ) : IRequest<IEnumerable<ClientForGettingDto>>;
}
