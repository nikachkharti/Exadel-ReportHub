using MediatR;
using ReportHub.Application.Features.Invoices.DTOs;

namespace ReportHub.Application.Features.Invoices.Queries
{
    public record GetAllInvoicesInADateRangeQuery
     (
         DateTime StartDate,
         DateTime EndDate,
         string ClientId,
         string CustomerId,
         int? PageNumber,
         int? PageSize,
         string SortingParameter,
         bool Ascending = false,
         CancellationToken CancellationToken = default
     ) : IRequest<IEnumerable<InvoiceForGettingDto>>;
}
