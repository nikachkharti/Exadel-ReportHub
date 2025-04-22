using MediatR;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Invoices.Commands;

public record CreateInvoiceCommand(string ClientId, string CustomerId, IList<string> items) : IRequest<Invoice>;
