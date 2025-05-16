using Newtonsoft.Json;
using ReportHub.Web.Models;
using ReportHub.Web.Models.Invoices;
using ReportHub.Web.Models.Invoices.Enum;
using ReportHub.Web.Services.Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportHub.Web.Services.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceApi _invoiceApi;

        public InvoiceService(IInvoiceApi invoiceApi)
        {
            _invoiceApi = invoiceApi;
        }

        public async Task<IEnumerable<InvoiceForGettingDto>> GetInvoicesAsync(int? page = 1, int? size = 10, string sortBy = "", bool ascending = true)
        {
            var response = await _invoiceApi.GetAllInvoicesAsync();

            if (!response.IsSuccess)
            {
                return Enumerable.Empty<InvoiceForGettingDto>();
            }

            return JsonConvert.DeserializeObject<IEnumerable<InvoiceForGettingDto>>(response.Result.ToString())
                ?? Enumerable.Empty<InvoiceForGettingDto>();
        }

        public async Task<IEnumerable<InvoiceForGettingDto>> GetInvoicesInDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            string clientId = "",
            string customerId = "",
            int? page = 1,
            int? size = 10,
            string sortBy = "",
            bool ascending = true)
        {
            var response = await _invoiceApi.GetAllInvoicesInADateRangeAsync(
                startDate,
                endDate,
                clientId,
                customerId,
                page,
                size,
                sortBy,
                ascending);

            if (!response.IsSuccess)
            {
                return Enumerable.Empty<InvoiceForGettingDto>();
            }

            return JsonConvert.DeserializeObject<IEnumerable<InvoiceForGettingDto>>(response.Result.ToString())
                ?? Enumerable.Empty<InvoiceForGettingDto>();
        }

        public async Task<InvoiceForGettingDto> GetInvoiceByIdAsync(string invoiceId)
        {
            var response = await _invoiceApi.GetInvoiceByIdAsync(invoiceId);

            if (!response.IsSuccess)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<InvoiceForGettingDto>(response.Result.ToString());
        }

        public async Task<byte[]> ExportInvoicesAsync(FileExportingType fileType)
        {
            try
            {
                return await _invoiceApi.ExportInvoicesAsync(fileType);
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }

        public async Task<byte[]> ExportInvoiceByIdAsync(string invoiceId, FileExportingType fileType)
        {
            try
            {
                return await _invoiceApi.ExportInvoiceByIdAsync(invoiceId, fileType);
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }

        public async Task<IEnumerable<InvoiceLogForGettingDto>> GetInvoiceLogsOfUserAsync(
            string userId,
            int? page = 1,
            int? size = 10,
            string sortBy = "",
            bool ascending = true)
        {
            var response = await _invoiceApi.GetAllInvoiceLogsOfUserAsync(
                userId,
                page,
                size,
                sortBy,
                ascending);

            if (!response.IsSuccess)
            {
                return Enumerable.Empty<InvoiceLogForGettingDto>();
            }

            return JsonConvert.DeserializeObject<IEnumerable<InvoiceLogForGettingDto>>(response.Result.ToString())
                ?? Enumerable.Empty<InvoiceLogForGettingDto>();
        }

        public async Task<StatisticsDto> GetInvoiceStatisticsAsync(GetStatistcsQuery query)
        {
            var response = await _invoiceApi.GetStatisticsAsync(query);

            if (!response.IsSuccess)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<StatisticsDto>(response.Result.ToString());
        }
    }
}