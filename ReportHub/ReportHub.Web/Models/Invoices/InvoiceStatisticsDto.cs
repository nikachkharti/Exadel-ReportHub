namespace ReportHub.Web.Models.Invoices
{
    public record InvoiceStatisticsDto
    (
        int TotalInvoiceCount,
        int PaidInvoiceCount,
        int UnpaidInvoiceCount,
        decimal TotalAmount,
        decimal PaidAmount,
        decimal UnpaidAmount,
        Dictionary<string, int> InvoicesByClient,
        Dictionary<DateTime, int> InvoicesByDate
    );
}