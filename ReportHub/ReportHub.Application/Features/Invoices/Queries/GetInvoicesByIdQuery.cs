using MediatR;
using ReportHub.Application.Features.Invoices.DTOs;

namespace ReportHub.Application.Features.Invoices.Queries;

public class GetInvoicesByIdQuery(string id) : IRequest<InvoiceForGettingDto>
{
    public string Id { get; } = id;
}
