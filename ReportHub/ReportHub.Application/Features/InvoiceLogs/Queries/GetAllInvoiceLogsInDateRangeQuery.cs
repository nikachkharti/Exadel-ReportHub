using MediatR;
using ReportHub.Application.Features.InvoiceLogs.DTOs;

namespace ReportHub.Application.Features.InvoiceLogs.Queries
{
    public record GetAllInvoiceLogsInDateRangeQuery(
        DateTime From,
        DateTime To,
        int? PageNumber,
        int? PageSize,
        string SortingParameter,
        bool Ascending = false,
        CancellationToken CancellationToken = default
    ) : IRequest<IEnumerable<InvoiceLogForGettingDto>>;
}
