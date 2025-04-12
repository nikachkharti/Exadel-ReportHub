namespace ReportHub.Application.Features.Invoices.DTOs;

public record InvoiceForGettingDto
(
    string Id,
    string ClientId,
    string CustomerId,
    DateTime IssueDate,
    DateTime DueDate,
    decimal Amount,
    string Currency,
    string PaymentStatus,
    List<string> ItemIds
);
