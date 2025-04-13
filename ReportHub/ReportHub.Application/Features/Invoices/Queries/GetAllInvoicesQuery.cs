using MediatR;
using ReportHub.Application.Features.Invoices.DTOs;

namespace ReportHub.Application.Features.Invoices.Queries;

public class GetAllInvoicesQuery : IRequest<IEnumerable<InvoiceForGettingDto>>
{
}
