using ReportHub.Web.Models.Invoices;
using ReportHub.Web.Models.Invoices.Enum;

namespace ReportHub.Web.Services.Invoice
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceForGettingDto>> GetInvoicesAsync(int? page = 1, int? size = 10, string sortBy = "", bool ascending = true);
        Task<IEnumerable<InvoiceForGettingDto>> GetInvoicesInDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            string clientId = "",
            string customerId = "",
            int? page = 1,
            int? size = 10,
            string sortBy = "",
            bool ascending = true);
        Task<InvoiceForGettingDto> GetInvoiceByIdAsync(string invoiceId);
        Task<byte[]> ExportInvoicesAsync(FileExportingType fileType);
        Task<byte[]> ExportInvoiceByIdAsync(string invoiceId, FileExportingType fileType);
        Task<IEnumerable<InvoiceLogForGettingDto>> GetInvoiceLogsOfUserAsync(
            string userId,
            int? page = 1,
            int? size = 10,
            string sortBy = "",
            bool ascending = true);
        Task<StatisticsDto> GetInvoiceStatisticsAsync(GetStatistcsQuery query);
    }
}