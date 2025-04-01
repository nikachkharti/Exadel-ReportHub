namespace ReportHub.Application.Features.Invoices.DTOs;

public class InvoiceDto
{
    public string InvoiceId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string PaymentStatus { get; set; }
}
