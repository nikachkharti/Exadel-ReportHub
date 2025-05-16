using Refit;
using ReportHub.Web.Models;
using ReportHub.Web.Models.Invoices;
using ReportHub.Web.Models.Invoices.Enum;

namespace ReportHub.Web.Services.Refit
{
    public interface IInvoiceApi
    {
        [Get("/api/invoice")]
        Task<EndpointResponse> GetAllInvoicesAsync();

        [Get("/api/invoice/{id}")]
        Task<EndpointResponse> GetInvoiceByIdAsync(string id);

        [Get("/api/invoice/date")]
        Task<EndpointResponse> GetAllInvoicesInADateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            string clientId = "",
            string customerId = "",
            int? pageNumber = 1,
            int? pageSize = 10,
            string sortingParameter = "",
            bool ascending = true);

        [Get("/api/invoice/export")]
        Task<byte[]> ExportInvoicesAsync(FileExportingType fileType);

        [Get("/api/invoice/{id}/export")]
        Task<byte[]> ExportInvoiceByIdAsync(string id, FileExportingType fileType);

        [Get("/api/invoice/logs")]
        Task<EndpointResponse> GetAllInvoiceLogsInDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            int? pageNumber = 1,
            int? pageSize = 10,
            string sortingParameter = "",
            bool ascending = true);

        [Get("/api/invoice/logs/user/{userId}")]
        Task<EndpointResponse> GetAllInvoiceLogsOfUserAsync(
            string userId,
            int? pageNumber = 1,
            int? pageSize = 10,
            string sortingParameter = "",
            bool ascending = true);

        [Get("/api/invoice/statistics")]
        Task<EndpointResponse> GetStatisticsAsync([AliasAs("query")] GetStatistcsQuery query);
    }
}