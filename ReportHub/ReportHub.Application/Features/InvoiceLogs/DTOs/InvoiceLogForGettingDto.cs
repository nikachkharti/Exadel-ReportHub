namespace ReportHub.Application.Features.InvoiceLogs.DTOs
{
    public record InvoiceLogForGettingDto(string Id, string UserId, string InvoiceId, DateTime TimeStamp, string Status);
}
