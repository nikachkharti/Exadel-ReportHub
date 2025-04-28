using CsvHelper;
using CsvHelper.Configuration;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Reflection;

namespace ReportHub.Infrastructure.Services.FileServices
{
    internal class CsvService : ICsvService
    {
        private readonly IClientRepository _clientRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IItemRepository _itemRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CsvService(
            IClientRepository clientRepo,
            ICustomerRepository customerRepo,
            IItemRepository itemRepo,
            IHttpContextAccessor httpContextAccessor)
        {
            _clientRepo = clientRepo;
            _customerRepo = customerRepo;
            _itemRepo = itemRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public IAsyncEnumerable<T> ReadAllAsync<T>(Stream stream, CancellationToken cancellationToken) where T : class
        {
            var reader = new StreamReader(stream);
            var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null
            });

            return GetRecordsOneByOne<T>(csv);
        }

        public async Task<Stream> WriteAllAsync<T>(IEnumerable<T> datas, IReadOnlyDictionary<string, object> statistics, CancellationToken token)
        {
            if (datas is IEnumerable<Invoice> invoices)
                return await CreateInvoicesCsvAsync(invoices, statistics, token);

            throw new NotSupportedException($"Data type {typeof(T).Name} is not supported for CSV generation.");
        }

        public async Task<Stream> WriteInvoiceAsync(Invoice invoice, IReadOnlyDictionary<string, object> statistics, CancellationToken token)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            return await CreateInvoiceCsvAsync(invoice, statistics, token);
        }

        private async Task<Stream> CreateInvoicesCsvAsync(IEnumerable<Invoice> invoices, IReadOnlyDictionary<string, object> statistics, CancellationToken token)
        {
            var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, leaveOpen: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            foreach (var invoice in invoices)
            {
                if (token.IsCancellationRequested) break;
                await WriteInvoiceSectionAsync(csv, invoice);
                csv.NextRecord(); 
                csv.NextRecord();
            }

            if (statistics?.Count > 0)
            {
                csv.NextRecord();
                WriteStatistics(statistics, csv);
            }

            await writer.FlushAsync();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private async Task<Stream> CreateInvoiceCsvAsync(Invoice invoice, IReadOnlyDictionary<string, object> statistics, CancellationToken token)
        {
            var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, leaveOpen: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            await WriteInvoiceSectionAsync(csv, invoice);

            if (statistics?.Count > 0)
            {
                csv.NextRecord();
                WriteStatistics(statistics, csv);
            }

            await writer.FlushAsync();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private async Task WriteInvoiceSectionAsync(CsvWriter csv, Invoice invoice)
        {
            var client = await _clientRepo.Get(c => c.Id == invoice.ClientId);
            var customer = await _customerRepo.Get(c => c.Id == invoice.CustomerId);
            var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id));

            csv.WriteField("Invoice Number");
            csv.WriteField(invoice.Id);
            csv.NextRecord();

            csv.WriteField("Customer");
            csv.WriteField(customer?.Name ?? "N/A");
            csv.NextRecord();

            csv.WriteField("Client");
            csv.WriteField(client?.Name ?? "N/A");
            csv.NextRecord();

            csv.WriteField("Issue Date");
            csv.WriteField(invoice.IssueDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture));
            csv.NextRecord();

            csv.WriteField("Due Date");
            csv.WriteField(invoice.DueDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture));
            csv.NextRecord();

            csv.WriteField("Payment Status");
            csv.WriteField(invoice.PaymentStatus);
            csv.NextRecord();

            csv.NextRecord(); 

            // Items header
            csv.WriteField("Item Name");
            csv.WriteField("Quantity");
            csv.WriteField("Price");
            csv.WriteField("Total");
            csv.NextRecord();

            decimal totalAmount = 0m;
            foreach (var item in items)
            {
                csv.WriteField(item.Name);
                csv.WriteField("1");
                csv.WriteField($"{item.Price:N2} {invoice.Currency}");
                csv.WriteField($"{item.Price:N2} {invoice.Currency}");
                totalAmount += item.Price;
                csv.NextRecord();
            }

            // Total row
            csv.WriteField("Total Amount");
            csv.WriteField($"{totalAmount:N2} {invoice.Currency}");
            csv.NextRecord();

            csv.NextRecord(); 

            // Download URL
            var request = _httpContextAccessor.HttpContext.Request;
            var url = $"{request.Scheme}://{request.Host}/invoice/download?invoiceId={invoice.Id}";
            csv.WriteField("Download URL");
            csv.WriteField(url);
            csv.NextRecord();
        }

        private static void WriteStatistics(IReadOnlyDictionary<string, object> statistics, CsvWriter csv)
        {
            foreach (var kv in statistics)
            {
                csv.WriteField(kv.Key);
                csv.WriteField(kv.Value?.ToString() ?? "");
                csv.NextRecord();
            }
        }

        private async IAsyncEnumerable<T> GetRecordsOneByOne<T>(CsvReader reader) where T : class
        {
            var properties = typeof(T).GetProperties();

            await reader.ReadAsync();
            reader.ReadHeader();

            while (await reader.ReadAsync())
            {
                var record = reader.GetRecord<T>();
                if (record != null)
                    yield return record;
            }
        }
    }
}
