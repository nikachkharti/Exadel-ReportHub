using System.Reflection;
using System.Runtime.CompilerServices;
using ClosedXML.Excel;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace ReportHub.Infrastructure.Services.FileServices
{
    internal class ExcelService : IExcelService
    {
        private readonly IClientRepository _clientRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IItemRepository _itemRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExcelService(
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

        public async Task<Stream> WriteAllAsync<T>(
            IEnumerable<T> datas,
            IReadOnlyDictionary<string, object> statistics,
            CancellationToken token)
        {
            if (datas is IEnumerable<Invoice> invoices)
                return await CreateInvoicesExcelAsync(invoices, statistics, token);

            throw new NotSupportedException(
                $"Data type {typeof(T).Name} is not supported for Excel generation.");
        }

        public async Task<Stream> WriteInvoiceAsync(
            Invoice invoice,
            IReadOnlyDictionary<string, object> statistics,
            CancellationToken token)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            return await CreateInvoiceExcelAsync(invoice, statistics, token);
        }

        public IAsyncEnumerable<T> ReadAllAsync<T>(Stream stream, CancellationToken cancellationToken) where T : class
        {
            return GetRecordsOneByOne<T>(stream, cancellationToken);
        }

        private async Task<Stream> CreateInvoicesExcelAsync(
            IEnumerable<Invoice> invoices,
            IReadOnlyDictionary<string, object> statistics,
            CancellationToken token)
        {
            var memoryStream = new MemoryStream();
            using var workbook = new XLWorkbook();

            // Add main invoice summary sheet
            var summarySheet = workbook.Worksheets.Add("All Invoices");
            await WriteInvoicesSummaryTableAsync(summarySheet, invoices);

            // Add enhanced statistics sheets
            var statsSheet = workbook.Worksheets.Add("Statistics");
            await WriteEnhancedStatisticsAsync(statsSheet, invoices, statistics);

            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        private async Task<Stream> CreateInvoiceExcelAsync(
            Invoice invoice,
            IReadOnlyDictionary<string, object> statistics,
            CancellationToken token)
        {
            var memoryStream = new MemoryStream();
            using var workbook = new XLWorkbook();

            // Main invoice sheet
            var worksheet = workbook.Worksheets.Add("Invoice Details");
            await WriteInvoiceTableAsync(worksheet, invoice);

            // Stats sheet if available
            var statsSheet = workbook.Worksheets.Add("Statistics");
            var invoices = new List<Invoice> { invoice };
            await WriteEnhancedStatisticsAsync(statsSheet, invoices, statistics);

            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        private async Task WriteInvoicesSummaryTableAsync(
            IXLWorksheet sheet,
            IEnumerable<Invoice> invoices)
        {
            // Create header row
            string[] headers = new[] {
                "Invoice ID", "Client", "Customer", "Issue Date", "Due Date",
                "Payment Status", "Currency", "Total Amount", "Days Until Due", "Created By"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
            }

            int row = 2;
            foreach (var invoice in invoices)
            {
                var client = await _clientRepo.Get(c => c.Id == invoice.ClientId);
                var customer = await _customerRepo.Get(c => c.Id == invoice.CustomerId);
                var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id));
                decimal totalAmount = items.Sum(i => i.Price);
                int daysUntilDue = (invoice.DueDate - DateTime.Today).Days;

                // Fill data row
                sheet.Cell(row, 1).Value = invoice.Id;
                sheet.Cell(row, 2).Value = client?.Name ?? "N/A";
                sheet.Cell(row, 3).Value = customer?.Name ?? "N/A";
                sheet.Cell(row, 4).Value = invoice.IssueDate.ToString("yyyy-MM-dd");
                sheet.Cell(row, 5).Value = invoice.DueDate.ToString("yyyy-MM-dd");
                sheet.Cell(row, 6).Value = invoice.PaymentStatus;
                sheet.Cell(row, 7).Value = invoice.Currency;
                sheet.Cell(row, 8).Value = totalAmount;
                sheet.Cell(row, 9).Value = daysUntilDue;

                row++;
            }

            // Format as table
            var range = sheet.Range(1, 1, row - 1, headers.Length);
            var table = range.CreateTable("AllInvoices");
            sheet.Columns().AdjustToContents();
        }

        private async Task WriteInvoiceTableAsync(
            IXLWorksheet sheet,
            Invoice invoice)
        {
            var client = await _clientRepo.Get(c => c.Id == invoice.ClientId);
            var customer = await _customerRepo.Get(c => c.Id == invoice.CustomerId);
            var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id));

            // Table 1: Invoice Header Information
            sheet.Cell(1, 1).Value = "Invoice Information";

            string[,] headerData = new string[,]
            {
                { "Invoice ID", invoice.Id },
                { "Client", client?.Name ?? "N/A" },
                { "Customer", customer?.Name ?? "N/A" },
                { "Issue Date", invoice.IssueDate.ToString("yyyy-MM-dd") },
                { "Due Date", invoice.DueDate.ToString("yyyy-MM-dd") },
                { "Payment Status", invoice.PaymentStatus },
                { "Days Until Due", (invoice.DueDate - DateTime.Today).Days.ToString() }
            };

            for (int i = 0; i < headerData.GetLength(0); i++)
            {
                sheet.Cell(i + 2, 1).Value = headerData[i, 0];
                sheet.Cell(i + 2, 2).Value = headerData[i, 1];
            }

            // Table 2: Invoice Items
            int itemStartRow = headerData.GetLength(0) + 4;
            sheet.Cell(itemStartRow, 1).Value = "Invoice Items";

            string[] itemHeaders = new[] { "Item Name", "Quantity", "Price", "Total" };
            for (int i = 0; i < itemHeaders.Length; i++)
            {
                sheet.Cell(itemStartRow + 1, i + 1).Value = itemHeaders[i];
            }

            int itemRow = itemStartRow + 2;
            decimal totalAmount = 0;
            foreach (var item in items)
            {
                sheet.Cell(itemRow, 1).Value = item.Name;
                sheet.Cell(itemRow, 2).Value = 1;
                sheet.Cell(itemRow, 3).Value = item.Price;
                sheet.Cell(itemRow, 4).Value = item.Price;

                totalAmount += item.Price;
                itemRow++;
            }

            // Total row
            sheet.Cell(itemRow + 1, 3).Value = "Total Amount:";
            sheet.Cell(itemRow + 1, 4).Value = totalAmount;
            sheet.Cell(itemRow + 2, 4).Value = $"({invoice.Currency})";

            // Format as tables
            var headerRange = sheet.Range(2, 1, headerData.GetLength(0) + 1, 2);
            var headerTable = headerRange.CreateTable("InvoiceInfo");

            var itemsRange = sheet.Range(itemStartRow + 1, 1, itemRow - 1, itemHeaders.Length);
            var itemsTable = itemsRange.CreateTable("InvoiceItems");

            sheet.Columns().AdjustToContents();
        }

        private async Task WriteEnhancedStatisticsAsync(
            IXLWorksheet sheet,
            IEnumerable<Invoice> invoices,
            IReadOnlyDictionary<string, object> baseStatistics)
        {
            // 1. General Statistics Table
            sheet.Cell(1, 1).Value = "General Statistics";
            sheet.Cell(2, 1).Value = "Metric";
            sheet.Cell(2, 2).Value = "Value";

            // Add base statistics first
            int row = 3;
            foreach (var kv in baseStatistics)
            {
                sheet.Cell(row, 1).Value = kv.Key;
                sheet.Cell(row, 2).Value = kv.Value?.ToString() ?? "N/A";
                row++;
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
                sheet.Cell(row, 1).Value = kv.Key;
                sheet.Cell(row, 2).Value = kv.Value?.ToString() ?? "N/A";
                row++;
            }

            var generalStatsRange = sheet.Range(2, 1, row - 1, 2);
            var generalStatsTable = generalStatsRange.CreateTable("GeneralStatistics");

            // 2. Payment Status Distribution Table
            row += 2; // Add space
            sheet.Cell(row, 1).Value = "Payment Status Distribution";
            row++;
            sheet.Cell(row, 1).Value = "Status";
            sheet.Cell(row, 2).Value = "Count";
            sheet.Cell(row, 3).Value = "Percentage";
            row++;

            var statusGroups = invoices.GroupBy(i => i.PaymentStatus)
                                      .Select(g => new { Status = g.Key, Count = g.Count() })
                                      .OrderByDescending(g => g.Count);

            int totalInvoices = invoices.Count();
            foreach (var group in statusGroups)
            {
                sheet.Cell(row, 1).Value = group.Status;
                sheet.Cell(row, 2).Value = group.Count;
                sheet.Cell(row, 3).Value = totalInvoices > 0
                    ? (double)group.Count / totalInvoices
                    : 0;
                sheet.Cell(row, 3).Style.NumberFormat.Format = "0.00%";
                row++;
            }

            var statusRange = sheet.Range(row - statusGroups.Count() - 1, 1, row - 1, 3);
            var statusTable = statusRange.CreateTable("StatusDistribution");

            // 3. Monthly Invoice Summary
            row += 2; // Add space
            sheet.Cell(row, 1).Value = "Monthly Invoice Summary";
            row++;
            sheet.Cell(row, 1).Value = "Month";
            sheet.Cell(row, 2).Value = "Count";
            sheet.Cell(row, 3).Value = "Total Amount";
            row++;

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
                sheet.Cell(row, 1).Value = month.YearMonth;
                sheet.Cell(row, 2).Value = month.Count;
                sheet.Cell(row, 3).Value = month.TotalAmount;
                sheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
                row++;
            }

            var monthlyRange = sheet.Range(row - monthlyResults.Length - 1, 1, row - 1, 3);
            var monthlyTable = monthlyRange.CreateTable("MonthlyInvoices");

            // 4. Client Summary
            if (invoices.Any())
            {
                row += 2; // Add space
                sheet.Cell(row, 1).Value = "Client Summary";
                row++;
                sheet.Cell(row, 1).Value = "Client";
                sheet.Cell(row, 2).Value = "Invoice Count";
                sheet.Cell(row, 3).Value = "Total Value";
                row++;

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
                    sheet.Cell(row, 1).Value = client.Client;
                    sheet.Cell(row, 2).Value = client.Count;
                    sheet.Cell(row, 3).Value = client.TotalAmount;
                    sheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
                    row++;
                }

                var clientRange = sheet.Range(row - clientResults.Length - 1, 1, row - 1, 3);
                var clientTable = clientRange.CreateTable("ClientSummary");
            }

            sheet.Columns().AdjustToContents();
        }

        private async Task<string> GetClientName(string clientId)
        {
            var client = await _clientRepo.Get(c => c.Id == clientId);
            return client?.Name ?? "Unknown";
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
                totalAmount += items.Sum(i => i.Price);
                count++;
            }

            return count > 0 ? totalAmount / count : 0;
        }

        private async Task<decimal> CalculateTotalOutstandingAmount(IEnumerable<Invoice> invoices)
        {
            decimal total = 0;

            foreach (var invoice in invoices.Where(i => i.PaymentStatus != "Paid"))
            {
                var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id));
                total += items.Sum(i => i.Price);
            }

            return total;
        }

        private async Task<decimal> CalculateTotalForInvoices(IEnumerable<Invoice> invoices)
        {
            decimal total = 0;

            foreach (var invoice in invoices)
            {
                var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id));
                total += items.Sum(i => i.Price);
            }

            return total;
        }

        private async IAsyncEnumerable<T> GetRecordsOneByOne<T>(
            Stream stream,
            [EnumeratorCancellation] CancellationToken cancellationToken) where T : class
        {
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault()
                ?? throw new InvalidOperationException("No worksheet found in Excel file");

            var properties = typeof(T).GetProperties();
            var headers = worksheet.Row(1).CellsUsed()
                .ToDictionary(cell => cell.Value.ToString(), cell => cell.Address.ColumnNumber, StringComparer.OrdinalIgnoreCase);

            int lastRow = worksheet.LastRowUsed().RowNumber();
            for (int row = 2; row <= lastRow; row++)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                var instance = Activator.CreateInstance<T>();
                foreach (var prop in properties)
                {
                    if (headers.TryGetValue(prop.Name, out int col))
                    {
                        var cellValue = worksheet.Cell(row, col).Value;
                    }
                }
                yield return instance;
            }
        }
    }
}