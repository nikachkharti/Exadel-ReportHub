using System.Reflection;
using System.Runtime.CompilerServices;
using ClosedXML.Excel;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Serilog;

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
            var worksheet = workbook.Worksheets.Add("Invoices");

            int row = 1;

            foreach (var invoice in invoices)
            {
                if (token.IsCancellationRequested) break;
                row = await WriteInvoiceSectionAsync(worksheet, invoice, row);
                row += 2; // Space between invoices
            }

            if (statistics?.Count > 0)
            {
                var statsSheet = workbook.Worksheets.Add("Statistics");
                WriteStatistics(statsSheet, statistics);
            }

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
            var worksheet = workbook.Worksheets.Add("Invoice");

            await WriteInvoiceSectionAsync(worksheet, invoice, startRow: 1);

            if (statistics?.Count > 0)
            {
                var statsSheet = workbook.Worksheets.Add("Statistics");
                WriteStatistics(statsSheet, statistics);
            }

            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        private async Task<int> WriteInvoiceSectionAsync(
            IXLWorksheet sheet,
            Invoice invoice,
            int startRow)
        {
            var client = await _clientRepo.Get(c => c.Id == invoice.ClientId);
            var customer = await _customerRepo.Get(c => c.Id == invoice.CustomerId);
            var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id));

            // Invoice Title
            sheet.Cell(startRow, 1).Value = $"Invoice #{invoice.Id}";
            sheet.Cell(startRow, 1).Style.Font.SetBold().Font.SetFontSize(16);
            sheet.Range(startRow, 1, startRow, 4).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            startRow += 2;

            // Invoice Details
            var details = new (string Label, string Value)[]
            {
                ("Customer", customer?.Name ?? "N/A"),
                ("Client", client?.Name ?? "N/A"),
                ("Issue Date", invoice.IssueDate.ToString("dd MMM yyyy")),
                ("Due Date", invoice.DueDate.ToString("dd MMM yyyy")),
                ("Payment Status", invoice.PaymentStatus)
            };

            foreach (var (label, value) in details)
            {
                sheet.Cell(startRow, 1).Value = label;
                sheet.Cell(startRow, 1).Style.Font.SetBold();
                sheet.Cell(startRow, 2).Value = value;
                startRow++;
            }

            startRow++;

            // Items Table
            var itemsHeader = new[] { "Item Name", "Quantity", "Price", "Total" };
            for (int col = 0; col < itemsHeader.Length; col++)
            {
                sheet.Cell(startRow, col + 1).Value = itemsHeader[col];
                sheet.Cell(startRow, col + 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                sheet.Cell(startRow, col + 1).Style.Font.SetBold();
                sheet.Cell(startRow, col + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            startRow++;

            decimal totalAmount = 0m;
            foreach (var item in items)
            {
                sheet.Cell(startRow, 1).Value = item.Name;
                sheet.Cell(startRow, 2).Value = 1;
                sheet.Cell(startRow, 3).Value = $"{item.Price:N2} {invoice.Currency}";
                sheet.Cell(startRow, 4).Value = $"{item.Price:N2} {invoice.Currency}";
                totalAmount += item.Price;
                startRow++;
            }

            // Total
            sheet.Cell(startRow, 1).Value = "Total Amount:";
            sheet.Cell(startRow, 1).Style.Font.SetBold();
            sheet.Range(startRow, 1, startRow, 3).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            sheet.Cell(startRow, 4).Value = $"{totalAmount:N2} {invoice.Currency}";
            sheet.Cell(startRow, 4).Style.Font.SetBold();
            sheet.Cell(startRow, 4).Style.Fill.SetBackgroundColor(XLColor.CornflowerBlue);
            startRow++;

            sheet.Columns().AdjustToContents();

            return startRow;
        }

        private static void WriteStatistics(
            IXLWorksheet sheet,
            IReadOnlyDictionary<string, object> statistics)
        {
            sheet.Cell(1, 1).Value = "Statistic";
            sheet.Cell(1, 2).Value = "Value";
            sheet.Row(1).Style.Font.SetBold();
            int row = 2;
            foreach (var kv in statistics)
            {
                sheet.Cell(row, 1).Value = kv.Key;
                sheet.Cell(row, 2).Value = kv.Value?.ToString() ?? "N/A";
                row++;
            }
            sheet.Columns().AdjustToContents();
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
                        if (!worksheet.Cell(row, col).IsEmpty())
                        {
                            try
                            {
                                object converted = Convert.ChangeType(cellValue, prop.PropertyType);
                                prop.SetValue(instance, converted);
                            }
                            catch
                            {
                                
                            }
                        }
                    }
                }
                yield return instance;
            }
        }
    }
}
