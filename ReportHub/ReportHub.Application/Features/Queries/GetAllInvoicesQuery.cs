using MediatR;
using ReportHub.Application.Features.DTOs;

namespace ReportHub.Application.Features.Queries;

public class GetAllInvoicesQuery : IRequest<IEnumerable<InvoiceDto>>
{
}
