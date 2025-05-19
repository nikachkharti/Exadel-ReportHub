using MediatR;
using ReportHub.Application.Features.Items.DTOs;

namespace ReportHub.Application.Features.Clients.Queries
{
    public record GetAllItemsOfClientQuery
        (
            string ClientId,
            int? PageNumber,
            int? PageSize,
            string SortingParameter,
            bool Ascending = false,
            CancellationToken CancellationToken = default
        ) : IRequest<IEnumerable<ItemForGettingDto>>;
}
