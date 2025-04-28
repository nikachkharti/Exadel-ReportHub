using MediatR;

namespace ReportHub.Application.Features.InvoiceLogs.Commands
{
    public record CreateInvoiceLogCommand(string UserId, string InvoiceId, DateTime TimeStamp, string Status) : IRequest<string>;
}
