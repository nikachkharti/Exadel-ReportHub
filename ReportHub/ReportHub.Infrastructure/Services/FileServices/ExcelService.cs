using System.Reflection;
using System.Runtime.CompilerServices;
using ClosedXML.Excel;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ReportHub.Infrastructure.Services.FileServices
{
    internal class ExcelService : IExcelService
    {
        private readonly IClientRepository _clientRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IItemRepository _itemRepo;
        private readonly IPlanRepository _planRepo; 
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExcelService(
            IClientRepository clientRepo,
            ICustomerRepository customerRepo,
            IItemRepository itemRepo,
            IPlanRepository planRepo, 
            IHttpContextAccessor httpContextAccessor)
        {
            _clientRepo = clientRepo;
            _customerRepo = customerRepo;
            _itemRepo = itemRepo;
            _planRepo = planRepo; 
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
            return GetRecordsOneByOneAsync<T>(stream, cancellationToken);
        }

        // FIX: Implemented the missing method
        private async IAsyncEnumerable<T> GetRecordsOneByOneAsync<T>(Stream stream,
            [EnumeratorCancellation] CancellationToken cancellationToken) where T : class
        {
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
                yield break;

            var headers = new List<string>();
            var properties = typeof(T).GetProperties();

            // Read headers from first row
            for (int col = 1; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
            {
                var header = worksheet.Cell(1, col).Value.ToString();
                headers.Add(header);
            }

            // Process data rows
            for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                var instance = Activator.CreateInstance<T>();

                for (int col = 1; col <= headers.Count; col++)
                {
                    var header = headers[col - 1];
                    var property = properties.FirstOrDefault(p =>
                        p.Name.Equals(header, StringComparison.OrdinalIgnoreCase));

                    if (property != null && property.CanWrite)
                    {
                        var cell = worksheet.Cell(row, col);
                        if (!cell.IsEmpty())
                        {
                            var value = cell.Value;
                            var convertedValue = Convert.ChangeType(value.ToString(), property.PropertyType);
                            property.SetValue(instance, convertedValue);
                        }
                    }
                }

                yield return instance;
            }
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

            // Add plans sheet
            var plansSheet = workbook.Worksheets.Add("Plan Details");
            await WritePlansTableAsync(plansSheet, token);

            // Add invoice-plan relationship sheet
            var relationshipSheet = workbook.Worksheets.Add("Invoice-Plan Relationship");
            await WriteInvoicePlanRelationshipAsync(relationshipSheet, invoices, token);

            // Add report metadata
            var metadataSheet = workbook.Worksheets.Add("Report Metadata");
            WriteReportMetadata(metadataSheet);

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

            // Add related plans for this invoice's items
            var planSheet = workbook.Worksheets.Add("Related Plans");
            await WriteRelatedPlansForInvoiceAsync(planSheet, invoice, token);

            // Add report metadata
            var metadataSheet = workbook.Worksheets.Add("Report Metadata");
            WriteReportMetadata(metadataSheet);

            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private async Task WriteRelatedPlansForInvoiceAsync(
            IXLWorksheet sheet,
            Invoice invoice,
            CancellationToken token)
        {
            // Create header row
            string[] headers = new[] {
                "Plan ID", "Item Name", "Start Date", "End Date", "Status", "Amount"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
            }

            int row = 2;
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
                        sheet.Cell(row, 1).Value = plan.Id;
                        sheet.Cell(row, 2).Value = item.Name;
                        sheet.Cell(row, 3).Value = plan.StartDate.ToString("yyyy-MM-dd");
                        sheet.Cell(row, 4).Value = plan.EndDate.ToString("yyyy-MM-dd");
                        sheet.Cell(row, 5).Value = plan.Status.ToString();
                        sheet.Cell(row, 6).Value = plan.Amount;
                        sheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

                        row++;
                    }
                }
            }

            if (hasData)
            {
                var range = sheet.Range(1, 1, row - 1, headers.Length);
                var table = range.CreateTable("RelatedPlans");
            }
            else
            {
                sheet.Cell(2, 1).Value = "No related plans found for this invoice";
            }

            sheet.Columns().AdjustToContents();
        }

        private void WriteReportMetadata(IXLWorksheet sheet)
        {
            // Write report generation information
            sheet.Cell(1, 1).Value = "Report Metadata";
            sheet.Cell(2, 1).Value = "Generation Date:";
            sheet.Cell(2, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Write user context information if available
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                sheet.Cell(3, 1).Value = "Generated By:";
                sheet.Cell(3, 2).Value = userName;
            }

            // Format as table
            var range = sheet.Range(1, 1, 3, 2);
            range.Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 14;
            sheet.Columns().AdjustToContents();
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

            // 5. Plan Status Distribution 
            row += 2; // Add space
            sheet.Cell(row, 1).Value = "Plan Status Distribution";
            row++;
            sheet.Cell(row, 1).Value = "Status";
            sheet.Cell(row, 2).Value = "Count";
            sheet.Cell(row, 3).Value = "Percentage";
            row++;

            var plans = await _planRepo.GetAll();
            var planStatusGroups = plans.GroupBy(p => p.Status)
                                       .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                                       .OrderByDescending(g => g.Count)
                                       .ToList();

            int totalPlans = plans.Count();
            foreach (var group in planStatusGroups)
            {
                sheet.Cell(row, 1).Value = group.Status;
                sheet.Cell(row, 2).Value = group.Count;
                sheet.Cell(row, 3).Value = totalPlans > 0
                    ? (double)group.Count / totalPlans
                    : 0;
                sheet.Cell(row, 3).Style.NumberFormat.Format = "0.00%";
                row++;
            }

            if (planStatusGroups.Any())
            {
                var planStatusRange = sheet.Range(row - planStatusGroups.Count - 1, 1, row - 1, 3);
                var planStatusTable = planStatusRange.CreateTable("PlanStatusDistribution");
            }

            sheet.Columns().AdjustToContents();
        }

        private async Task WritePlansTableAsync(IXLWorksheet sheet, CancellationToken token)
        {
            // Create header row for plans
            string[] headers = new[] {
                "Plan ID", "Client", "Item", "Amount", "Start Date", "End Date", "Status", "Duration (Days)"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
            }

            // Get all plans
            var plans = await _planRepo.GetAll(token);

            int row = 2;
            foreach (var plan in plans)
            {
                var client = await _clientRepo.Get(c => c.Id == plan.ClientId, token);
                var item = await _itemRepo.Get(i => i.Id == plan.ItemId, token);
                int duration = (plan.EndDate - plan.StartDate).Days;

                // Fill data row
                sheet.Cell(row, 1).Value = plan.Id;
                sheet.Cell(row, 2).Value = client?.Name ?? "N/A";
                sheet.Cell(row, 3).Value = item?.Name ?? "N/A";
                sheet.Cell(row, 4).Value = plan.Amount;
                sheet.Cell(row, 5).Value = plan.StartDate.ToString("yyyy-MM-dd");
                sheet.Cell(row, 6).Value = plan.EndDate.ToString("yyyy-MM-dd");
                sheet.Cell(row, 7).Value = plan.Status.ToString();
                sheet.Cell(row, 8).Value = duration;

                row++;
            }

            // Format as table if there are plans
            if (plans.Any())
            {
                var range = sheet.Range(1, 1, row - 1, headers.Length);
                var table = range.CreateTable("AllPlans");

                // Format money columns
                sheet.Column(4).Style.NumberFormat.Format = "#,##0.00";
            }
            else
            {
                sheet.Cell(2, 1).Value = "No plans available";
            }

            sheet.Columns().AdjustToContents();
        }

        private async Task WriteInvoicePlanRelationshipAsync(
            IXLWorksheet sheet,
            IEnumerable<Invoice> invoices,
            CancellationToken token)
        {
            // Create header row
            string[] headers = new[] {
                "Invoice ID", "Client", "Item", "Plan ID", "Plan Status", "Plan Start", "Plan End"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
            }

            int row = 2;
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
                            sheet.Cell(row, 1).Value = invoice.Id;
                            sheet.Cell(row, 2).Value = client?.Name ?? "N/A";
                            sheet.Cell(row, 3).Value = item.Name;
                            sheet.Cell(row, 4).Value = plan.Id;  // FIX: Removed sheet.Cell(row, 4).Value = plan.sheet
                            sheet.Cell(row, 5).Value = plan.Status.ToString();
                            sheet.Cell(row, 6).Value = plan.StartDate.ToString("yyyy-MM-dd");
                            sheet.Cell(row, 7).Value = plan.EndDate.ToString("yyyy-MM-dd");

                            row++;
                        }
                    }
                }
            }

            // Format as table if there is data
            if (hasData)
            {
                var range = sheet.Range(1, 1, row - 1, headers.Length);
                var table = range.CreateTable("InvoicePlanRelationship");
            }
            else
            {
                sheet.Cell(2, 1).Value = "No invoice-plan relationships found";
            }

            sheet.Columns().AdjustToContents();
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
                decimal invoiceTotal = items.Sum(i => i.Price);
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
                    decimal invoiceTotal = items.Sum(i => i.Price);
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
                total += items.Sum(i => i.Price);
            }

            return total;
        }
    }
}