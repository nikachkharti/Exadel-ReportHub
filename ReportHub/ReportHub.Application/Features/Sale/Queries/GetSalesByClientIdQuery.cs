using MediatR;
using ReportHub.Application.Features.Sale.DTOs;

namespace ReportHub.Application.Features.Sale.Queries
{
    public record GetSalesByClientIdQuery
    (
        string Clientid,
        int? PageNumber,
        int? PageSize,
        string SortingParameter,
        bool Ascending = false,
        CancellationToken CancellationToken = default
    ) : IRequest<IEnumerable<SaleForGettingDto>>;
}
