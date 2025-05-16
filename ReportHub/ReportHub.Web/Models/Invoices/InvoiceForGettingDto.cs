namespace ReportHub.Web.Models.Invoices
{
    public record InvoiceForGettingDto
    (
        string Id,
        string Number,
        DateTime Date,
        decimal Amount,
        string ClientId,
        string ClientName,
        string CustomerId,
        string CustomerName,
        bool IsPaid,
        bool IsDeleted
    );
}