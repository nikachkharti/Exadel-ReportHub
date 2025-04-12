using MediatR;
using ReportHub.Application.Features.Clients.DTOs;

namespace ReportHub.Application.Features.Clients.Queries
{
    public record GetAllClientsQuery
    (
        int? PageNumber = 1,
        int? PageSize = 10,
        string SortingParameter = "",
        bool Descending = false,
        CancellationToken CancellationToken = default
    ) : IRequest<IEnumerable<ClientForGettingDto>>;
}
