namespace ReportHub.Web.Models.Invoices;

public record InvoiceLogForGettingDto(string Id, string UserId, string InvoiceId, DateTime TimeStamp, string Status, bool IsDeleted);
