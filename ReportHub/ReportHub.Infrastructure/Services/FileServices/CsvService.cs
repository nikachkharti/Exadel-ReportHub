using CsvHelper;
using CsvHelper.Configuration;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Contracts.CurrencyContracts;
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
        private readonly IPlanRepository _planRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExchangeCurrencyService _exchangeCurrencyService;

        public CsvService(
            IClientRepository clientRepo,
            ICustomerRepository customerRepo,
            IItemRepository itemRepo,
            IPlanRepository planRepo,
            IHttpContextAccessor httpContextAccessor,
            IExchangeCurrencyService exchangeCurrencyService)
        {
            _clientRepo = clientRepo;
            _customerRepo = customerRepo;
            _itemRepo = itemRepo;
            _planRepo = planRepo;
            _httpContextAccessor = httpContextAccessor;
            _exchangeCurrencyService = exchangeCurrencyService;
        }

        public IAsyncEnumerable<T> ReadAllAsync<T>(Stream stream, CancellationToken cancellationToken) where T : class
        {
            var reader = new StreamReader(stream);
            var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null
            });

            return GetRecordsOneByOne<T>(csv, cancellationToken);
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

        private async Task<Stream> CreateInvoicesCsvAsync(
            IEnumerable<Invoice> invoices,
            IReadOnlyDictionary<string, object> statistics,
            CancellationToken token)
        {
            var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, leaveOpen: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write invoice summary table
            await WriteInvoicesSummaryTableAsync(csv, invoices, token);
            csv.NextRecord();
            csv.NextRecord();

            // Write enhanced statistics
            await WriteEnhancedStatisticsAsync(csv, invoices, statistics, token);
            csv.NextRecord();
            csv.NextRecord();

            // Write plans table
            await WritePlansTableAsync(csv, token);
            csv.NextRecord();
            csv.NextRecord();

            // Write invoice-plan relationship
            await WriteInvoicePlanRelationshipAsync(csv, invoices, token);
            csv.NextRecord();
            csv.NextRecord();

            // Write report metadata
            WriteReportMetadata(csv);

            await writer.FlushAsync();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private async Task<Stream> CreateInvoiceCsvAsync(
            Invoice invoice,
            IReadOnlyDictionary<string, object> statistics,
            CancellationToken token)
        {
            var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, leaveOpen: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write invoice table
            await WriteInvoiceTableAsync(csv, invoice, token);
            csv.NextRecord();
            csv.NextRecord();

            // Write stats
            var invoices = new List<Invoice> { invoice };
            await WriteEnhancedStatisticsAsync(csv, invoices, statistics, token);
            csv.NextRecord();
            csv.NextRecord();

            // Write related plans for this invoice
            await WriteRelatedPlansForInvoiceAsync(csv, invoice, token);
            csv.NextRecord();
            csv.NextRecord();

            // Write report metadata
            WriteReportMetadata(csv);

            await writer.FlushAsync();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private async Task WriteInvoiceTableAsync(
            CsvWriter csv,
            Invoice invoice,
            CancellationToken token)
        {
            var client = await _clientRepo.Get(c => c.Id == invoice.ClientId, token);
            var customer = await _customerRepo.Get(c => c.Id == invoice.CustomerId, token);
            var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id), token);

            // Invoice Information Header
            csv.WriteField("Invoice Information");
            csv.NextRecord();

            // Invoice Header Data
            csv.WriteField("Invoice ID");
            csv.WriteField(invoice.Id);
            csv.NextRecord();

            csv.WriteField("Client");
            csv.WriteField(client?.Name ?? "N/A");
            csv.NextRecord();

            csv.WriteField("Customer");
            csv.WriteField(customer?.Name ?? "N/A");
            csv.NextRecord();

            csv.WriteField("Issue Date");
            csv.WriteField(invoice.IssueDate.ToString("yyyy-MM-dd"));
            csv.NextRecord();

            csv.WriteField("Due Date");
            csv.WriteField(invoice.DueDate.ToString("yyyy-MM-dd"));
            csv.NextRecord();

            csv.WriteField("Payment Status");
            csv.WriteField(invoice.PaymentStatus);
            csv.NextRecord();

            csv.WriteField("Days Until Due");
            csv.WriteField((invoice.DueDate - DateTime.Today).Days.ToString());
            csv.NextRecord();

            csv.NextRecord();

            // Invoice Items
            csv.WriteField("Invoice Items");
            csv.NextRecord();

            csv.WriteField("Item Name");
            csv.WriteField("Quantity");
            csv.WriteField("Price (USD)");
            csv.WriteField("Total (USD)");
            csv.NextRecord();

            decimal totalAmount = 0;
            foreach (var item in items)
            {
                var priceInUsd = await ConvertToUsd(item.Price, item.Currency);
                csv.WriteField(item.Name);
                csv.WriteField(1);
                csv.WriteField(priceInUsd);
                csv.WriteField(priceInUsd);
                totalAmount += priceInUsd;
                csv.NextRecord();
            }

            // Total row
            csv.WriteField("");
            csv.WriteField("");
            csv.WriteField("Total Amount (USD):");
            csv.WriteField(totalAmount);
            csv.NextRecord();
        }

        private async Task WriteInvoicesSummaryTableAsync(
            CsvWriter csv,
            IEnumerable<Invoice> invoices,
            CancellationToken token)
        {
            // Create header row
            csv.WriteField("All Invoices");
            csv.NextRecord();

            csv.WriteField("Invoice ID");
            csv.WriteField("Client");
            csv.WriteField("Customer");
            csv.WriteField("Issue Date");
            csv.WriteField("Due Date");
            csv.WriteField("Payment Status");
            csv.WriteField("Original Currency");
            csv.WriteField("Total Amount (USD)");
            csv.WriteField("Days Until Due");
            csv.NextRecord();

            foreach (var invoice in invoices)
            {
                var client = await _clientRepo.Get(c => c.Id == invoice.ClientId, token);
                var customer = await _customerRepo.Get(c => c.Id == invoice.CustomerId, token);
                var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id), token);
                decimal totalAmount = 0;
                foreach (var item in items)
                {
                    totalAmount += await ConvertToUsd(item.Price, item.Currency);
                }
                int daysUntilDue = (invoice.DueDate - DateTime.Today).Days;

                // Fill data row
                csv.WriteField(invoice.Id);
                csv.WriteField(client?.Name ?? "N/A");
                csv.WriteField(customer?.Name ?? "N/A");
                csv.WriteField(invoice.IssueDate.ToString("yyyy-MM-dd"));
                csv.WriteField(invoice.DueDate.ToString("yyyy-MM-dd"));
                csv.WriteField(invoice.PaymentStatus);
                csv.WriteField(invoice.Currency);
                csv.WriteField(totalAmount);
                csv.WriteField(daysUntilDue);
                csv.NextRecord();
            }
        }

        private async Task WriteEnhancedStatisticsAsync(
            CsvWriter csv,
            IEnumerable<Invoice> invoices,
            IReadOnlyDictionary<string, object> baseStatistics,
            CancellationToken token)
        {
            // 1. General Statistics Table
            csv.WriteField("General Statistics");
            csv.NextRecord();
            csv.WriteField("Metric");
            csv.WriteField("Value");
            csv.NextRecord();

            // Add base statistics first
            foreach (var kv in baseStatistics)
            {
                csv.WriteField(kv.Key);
                csv.WriteField(kv.Value?.ToString() ?? "N/A");
                csv.NextRecord();
            }

            // Add additional general statistics
            var now = DateTime.Today;
            var additionalStats = new Dictionary<string, object>
            {
                { "Average Invoice Amount", await CalculateAverageInvoiceAmount(invoices) },
                { "Invoices Due This Week", invoices.Count(i => (i.DueDate - now).Days <= 7 && (i.DueDate - now).Days >= 0 && i.PaymentStatus != "Paid") },
                { "Invoices Overdue", invoices.Count(i => i.DueDate < now && i.PaymentStatus != "Paid") },
                { "Total Outstanding Amount", await CalculateTotalOutstandingAmount(invoices) }
            };

            foreach (var kv in additionalStats)
            {
                csv.WriteField(kv.Key);
                csv.WriteField(kv.Value?.ToString() ?? "N/A");
                csv.NextRecord();
            }

            csv.NextRecord();

            // 2. Payment Status Distribution Table
            csv.WriteField("Payment Status Distribution");
            csv.NextRecord();
            csv.WriteField("Status");
            csv.WriteField("Count");
            csv.WriteField("Percentage");
            csv.NextRecord();

            var statusGroups = invoices.GroupBy(i => i.PaymentStatus)
                                      .Select(g => new { Status = g.Key, Count = g.Count() })
                                      .OrderByDescending(g => g.Count);

            int totalInvoices = invoices.Count();
            foreach (var group in statusGroups)
            {
                csv.WriteField(group.Status);
                csv.WriteField(group.Count);
                csv.WriteField(totalInvoices > 0
                    ? $"{(double)group.Count / totalInvoices:P2}"
                    : "0.00%");
                csv.NextRecord();
            }

            csv.NextRecord();

            // 3. Monthly Invoice Summary
            csv.WriteField("Monthly Invoice Summary");
            csv.NextRecord();
            csv.WriteField("Month");
            csv.WriteField("Count");
            csv.WriteField("Total Amount");
            csv.NextRecord();

            var monthlyGroups = invoices.GroupBy(i => new { i.IssueDate.Year, i.IssueDate.Month })
                                       .Select(async g => new {
                                           YearMonth = $"{g.Key.Year}-{g.Key.Month:D2}",
                                           Count = g.Count(),
                                           TotalAmount = await CalculateTotalForInvoices(g)
                                       })
                                       .ToList();

            var monthlyResults = await Task.WhenAll(monthlyGroups);
            foreach (var month in monthlyResults.OrderBy(m => m.YearMonth))
            {
                csv.WriteField(month.YearMonth);
                csv.WriteField(month.Count);
                csv.WriteField($"{month.TotalAmount:N2}");
                csv.NextRecord();
            }

            csv.NextRecord();

            // 4. Client Summary
            if (invoices.Any())
            {
                csv.WriteField("Client Summary");
                csv.NextRecord();
                csv.WriteField("Client");
                csv.WriteField("Invoice Count");
                csv.WriteField("Total Value");
                csv.NextRecord();

                var clientGroups = invoices.GroupBy(i => i.ClientId)
                                         .Select(async g => new {
                                             ClientId = g.Key,
                                             Client = await GetClientName(g.Key),
                                             Count = g.Count(),
                                             TotalAmount = await CalculateTotalForInvoices(g)
                                         })
                                         .ToList();

                var clientResults = await Task.WhenAll(clientGroups);
                foreach (var client in clientResults.OrderByDescending(c => c.TotalAmount))
                {
                    csv.WriteField(client.Client);
                    csv.WriteField(client.Count);
                    csv.WriteField($"{client.TotalAmount:N2}");
                    csv.NextRecord();
                }

                csv.NextRecord();
            }

            // 5. Plan Status Distribution 
            csv.WriteField("Plan Status Distribution");
            csv.NextRecord();
            csv.WriteField("Status");
            csv.WriteField("Count");
            csv.WriteField("Percentage");
            csv.NextRecord();

            var plans = await _planRepo.GetAll(token);
            var planStatusGroups = plans.GroupBy(p => p.Status)
                                       .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                                       .OrderByDescending(g => g.Count)
                                       .ToList();

            int totalPlans = plans.Count();
            foreach (var group in planStatusGroups)
            {
                csv.WriteField(group.Status);
                csv.WriteField(group.Count);
                csv.WriteField(totalPlans > 0
                    ? $"{(double)group.Count / totalPlans:P2}"
                    : "0.00%");
                csv.NextRecord();
            }
        }

        private async Task WritePlansTableAsync(CsvWriter csv, CancellationToken token)
        {
            // Create header row for plans
            csv.WriteField("Plan Details");
            csv.NextRecord();

            csv.WriteField("Plan ID");
            csv.WriteField("Client");
            csv.WriteField("Item");
            csv.WriteField("Amount (USD)");
            csv.WriteField("Start Date");
            csv.WriteField("End Date");
            csv.WriteField("Status");
            csv.WriteField("Duration (Days)");
            csv.NextRecord();

            // Get all plans
            var plans = await _planRepo.GetAll(token);

            if (plans.Any())
            {
                foreach (var plan in plans)
                {
                    var client = await _clientRepo.Get(c => c.Id == plan.ClientId, token);
                    var item = await _itemRepo.Get(i => i.Id == plan.ItemId, token);
                    int duration = (plan.EndDate - plan.StartDate).Days;

                    // Convert amount to USD
                    var amountInUsd = await ConvertToUsd(plan.Amount, item?.Currency ?? "USD");

                    // Fill data row
                    csv.WriteField(plan.Id);
                    csv.WriteField(client?.Name ?? "N/A");
                    csv.WriteField(item?.Name ?? "N/A");
                    csv.WriteField(amountInUsd);
                    csv.WriteField(plan.StartDate.ToString("yyyy-MM-dd"));
                    csv.WriteField(plan.EndDate.ToString("yyyy-MM-dd"));
                    csv.WriteField(plan.Status.ToString());
                    csv.WriteField(duration);
                    csv.NextRecord();
                }
            }
            else
            {
                csv.WriteField("No plans available");
                csv.NextRecord();
            }
        }

        private async Task WriteInvoicePlanRelationshipAsync(
            CsvWriter csv,
            IEnumerable<Invoice> invoices,
            CancellationToken token)
        {
            // Create header row
            csv.WriteField("Invoice-Plan Relationship");
            csv.NextRecord();

            csv.WriteField("Invoice ID");
            csv.WriteField("Client");
            csv.WriteField("Item");
            csv.WriteField("Plan ID");
            csv.WriteField("Plan Status");
            csv.WriteField("Plan Start");
            csv.WriteField("Plan End");
            csv.NextRecord();

            bool hasData = false;

            // For each invoice
            foreach (var invoice in invoices)
            {
                var client = await _clientRepo.Get(c => c.Id == invoice.ClientId, token);
                var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id), token);

                // For each item in the invoice
                foreach (var item in items)
                {
                    // Find plans for this item
                    var itemPlans = await _planRepo.GetAll(
                        p => p.ItemId == item.Id && p.ClientId == invoice.ClientId,
                        token);

                    if (itemPlans.Any())
                    {
                        hasData = true;

                        foreach (var plan in itemPlans)
                        {
                            csv.WriteField(invoice.Id);
                            csv.WriteField(client?.Name ?? "N/A");
                            csv.WriteField(item.Name);
                            csv.WriteField(plan.Id);
                            csv.WriteField(plan.Status.ToString());
                            csv.WriteField(plan.StartDate.ToString("yyyy-MM-dd"));
                            csv.WriteField(plan.EndDate.ToString("yyyy-MM-dd"));
                            csv.NextRecord();
                        }
                    }
                }
            }

            if (!hasData)
            {
                csv.WriteField("No invoice-plan relationships found");
                csv.NextRecord();
            }
        }

        private async Task WriteRelatedPlansForInvoiceAsync(
            CsvWriter csv,
            Invoice invoice,
            CancellationToken token)
        {
            // Create header row
            csv.WriteField("Related Plans");
            csv.NextRecord();

            csv.WriteField("Plan ID");
            csv.WriteField("Item Name");
            csv.WriteField("Start Date");
            csv.WriteField("End Date");
            csv.WriteField("Status");
            csv.WriteField("Amount (USD)");
            csv.NextRecord();

            bool hasData = false;
            var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id), token);

            foreach (var item in items)
            {
                var plans = await _planRepo.GetAll(
                    p => p.ClientId == invoice.ClientId && p.ItemId == item.Id,
                    token);

                if (plans.Any())
                {
                    hasData = true;

                    foreach (var plan in plans)
                    {
                        // Convert amount to USD
                        var amountInUsd = await ConvertToUsd(plan.Amount, item.Currency);

                        csv.WriteField(plan.Id);
                        csv.WriteField(item.Name);
                        csv.WriteField(plan.StartDate.ToString("yyyy-MM-dd"));
                        csv.WriteField(plan.EndDate.ToString("yyyy-MM-dd"));
                        csv.WriteField(plan.Status.ToString());
                        csv.WriteField(amountInUsd);
                        csv.NextRecord();
                    }
                }
            }

            if (!hasData)
            {
                csv.WriteField("No related plans found for this invoice");
                csv.NextRecord();
            }
        }

        private void WriteReportMetadata(CsvWriter csv)
        {
            // Write report generation information
            csv.WriteField("Report Metadata");
            csv.NextRecord();

            csv.WriteField("Generation Date:");
            csv.WriteField(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            csv.NextRecord();

            // Write user context information if available
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                csv.WriteField("Generated By:");
                csv.WriteField(userName);
                csv.NextRecord();
            }
        }

        private async Task<decimal> ConvertToUsd(decimal amount, string fromCurrency)
        {
            if (fromCurrency == "USD") return amount;
            var rate = await _exchangeCurrencyService.GetCurrencyAsync(fromCurrency, "USD");
            return amount * rate;
        }

        private async Task<decimal> CalculateAverageInvoiceAmount(IEnumerable<Invoice> invoices)
        {
            if (!invoices.Any())
                return 0;

            decimal totalAmount = 0;
            int count = 0;

            foreach (var invoice in invoices)
            {
                var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id));
                decimal invoiceTotal = 0;
                foreach (var item in items)
                {
                    invoiceTotal += await ConvertToUsd(item.Price, item.Currency);
                }
                totalAmount += invoiceTotal;
                count++;
            }

            return count > 0 ? totalAmount / count : 0;
        }

        private async Task<decimal> CalculateTotalOutstandingAmount(IEnumerable<Invoice> invoices)
        {
            decimal outstandingAmount = 0;

            foreach (var invoice in invoices)
            {
                if (invoice.PaymentStatus != "Paid")
                {
                    var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id));
                    decimal invoiceTotal = 0;
                    foreach (var item in items)
                    {
                        invoiceTotal += await ConvertToUsd(item.Price, item.Currency);
                    }
                    outstandingAmount += invoiceTotal;
                }
            }

            return outstandingAmount;
        }

        private async Task<string> GetClientName(string clientId)
        {
            var client = await _clientRepo.Get(c => c.Id == clientId);
            return client?.Name ?? "Unknown Client";
        }

        private async Task<decimal> CalculateTotalForInvoices(IEnumerable<Invoice> invoices)
        {
            decimal total = 0;

            foreach (var invoice in invoices)
            {
                var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id));
                foreach (var item in items)
                {
                    total += await ConvertToUsd(item.Price, item.Currency);
                }
            }

            return total;
        }

        private async IAsyncEnumerable<T> GetRecordsOneByOne<T>(
            CsvReader reader,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken) where T : class
        {
            await reader.ReadAsync();
            reader.ReadHeader();

            while (await reader.ReadAsync())
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                var record = reader.GetRecord<T>();
                if (record != null)
                    yield return record;
            }
        }
    }
}