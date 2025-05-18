using System.Globalization;
using iText = iTextSharp.text;
using iTextPdf = iTextSharp.text.pdf;

using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Contracts.CurrencyContracts;
using ReportHub.Domain.Entities;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using iTextSharp.text;
using iTextSharp.text.pdf.draw;

namespace ReportHub.Infrastructure.Services.FileServices
{
    internal class PdfService : IPdfService
    {
        private readonly IClientRepository _clientRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IItemRepository _itemRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExchangeCurrencyService _exchangeCurrencyService;

        // Added color constants
        private readonly BaseColor LightGray = BaseColor.LIGHT_GRAY;
        private readonly BaseColor DarkGray = new BaseColor(51, 51, 51);
        private readonly BaseColor BrandColor = new BaseColor(52, 152, 219);

        public PdfService(
            IClientRepository clientRepo,
            ICustomerRepository customerRepo,
            IItemRepository itemRepo,
            IHttpContextAccessor httpContextAccessor,
            IExchangeCurrencyService exchangeCurrencyService)
        {
            _clientRepo = clientRepo;
            _customerRepo = customerRepo;
            _itemRepo = itemRepo;
            _httpContextAccessor = httpContextAccessor;
            _exchangeCurrencyService = exchangeCurrencyService;
        }

        private async Task<decimal> ConvertToUsd(decimal amount, string fromCurrency)
        {
            if (fromCurrency == "USD") return amount;
            var rate = await _exchangeCurrencyService.GetCurrencyAsync(fromCurrency, "USD");
            return amount * rate;
        }

        public async Task<Stream> WriteAllAsync<T>(
            IEnumerable<T> datas,
            IReadOnlyDictionary<string, object> statistics,
            CancellationToken token)
        {
            if (datas is IEnumerable<Invoice> invoices)
                return await CreateInvoicesPdfAsync(invoices, statistics, token);

            throw new NotSupportedException(
              $"Data type {typeof(T).Name} is not supported for PDF generation");
        }

        public async Task<Stream> WriteInvoiceAsync(
            Invoice invoice,
            IReadOnlyDictionary<string, object> statistics,
            CancellationToken token)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            return await CreateInvoicePdfAsync(invoice, statistics, token);
        }

        private async Task<Stream> CreateInvoicesPdfAsync(
            IEnumerable<Invoice> invoices,
            IReadOnlyDictionary<string, object> statistics,
            CancellationToken token)
        {
            var ms = new MemoryStream();
            var doc = new iText.Document(iText.PageSize.A4, 50, 50, 50, 50);
            var writer = iTextPdf.PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;
            doc.Open();

            foreach (var invoice in invoices)
            {
                if (token.IsCancellationRequested) break;
                await AddInvoicePageToDocumentAsync(doc, invoice, statistics);
                doc.NewPage();
            }

            if (statistics?.Count > 0)
            {
                AddStatisticsPage(doc, statistics);
            }

            doc.Close();
            ms.Position = 0;
            return ms;
        }

        private async Task<Stream> CreateInvoicePdfAsync(
            Invoice invoice,
            IReadOnlyDictionary<string, object> statistics,
            CancellationToken token)
        {
            var ms = new MemoryStream();
            var doc = new iText.Document(iText.PageSize.A4, 50, 50, 50, 50);
            var writer = iTextPdf.PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;
            doc.Open();

            await AddInvoicePageToDocumentAsync(doc, invoice, statistics);

            if (statistics?.Count > 0)
            {
                doc.NewPage();
                AddStatisticsPage(doc, statistics);
            }

            doc.Close();
            ms.Position = 0;
            return ms;
        }

        private async Task AddInvoicePageToDocumentAsync(
            iText.Document document,
            Invoice invoice,
            IReadOnlyDictionary<string, object> statistics)
        {
            Console.WriteLine($"[PDF Export] Start for InvoiceId: {invoice?.Id}");
            var client = await _clientRepo.Get(c => c.Id == invoice.ClientId);
            if (client == null)
            {
                Console.WriteLine($"[PDF Export] Client not found: {invoice.ClientId}");
                throw new Exception($"Client not found for invoice {invoice.Id}");
            }
            Console.WriteLine($"[PDF Export] Client found: {client.Id}");

            var customer = await _customerRepo.Get(c => c.Id == invoice.CustomerId);
            if (customer == null)
            {
                Console.WriteLine($"[PDF Export] Customer not found: {invoice.CustomerId}");
                throw new Exception($"Customer not found for invoice {invoice.Id}");
            }
            Console.WriteLine($"[PDF Export] Customer found: {customer.Id}");

            var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id));
            if (items == null || !items.Any())
            {
                Console.WriteLine($"[PDF Export] No items found for invoice {invoice.Id}. ItemIds: {string.Join(",", invoice.ItemIds)}");
                throw new Exception($"No items found for invoice {invoice.Id}");
            }
            Console.WriteLine($"[PDF Export] Items found: {string.Join(",", items.Select(i => i.Id))}");

            if (_httpContextAccessor.HttpContext == null)
            {
                Console.WriteLine($"[PDF Export] HttpContext is null");
                throw new Exception("No HTTP context available for PDF export.");
            }
            Console.WriteLine($"[PDF Export] HttpContext is present");

            // Enhanced Title Section
            var titleFont = iText.FontFactory.GetFont(
                iText.FontFactory.HELVETICA_BOLD, 18, DarkGray);

            var title = new iText.Paragraph($"Invoice Number: {invoice.Id}", titleFont)
            {
                Alignment = iText.Element.ALIGN_CENTER,
                SpacingAfter = 30,
                Leading = 1.5f
            };
            title.Add(new Chunk(new DottedLineSeparator()));
            document.Add(title);

            // Details Table
            var details = new iTextPdf.PdfPTable(2) { WidthPercentage = 100f };
            details.SetWidths(new float[] { 1f, 3f });
            details.DefaultCell.Border = PdfPCell.NO_BORDER;
            details.DefaultCell.Padding = 10;
            details.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

            AddCell(details, "Customer:", true);
            AddCell(details, customer?.Name ?? "N/A");

            AddCell(details, "Client:", true);
            AddCell(details, client?.Name ?? "N/A");

            AddCell(details, "Issue Date:", true);
            AddCell(details, invoice.IssueDate.ToString("dd MMM yyyy"));

            AddCell(details, "Due Date:", true);
            AddCell(details, invoice.DueDate.ToString("dd MMM yyyy"));

            AddCell(details, "Payment Status:", true);
            AddCell(details, invoice.PaymentStatus, textColor: GetStatusColor(invoice.PaymentStatus));

            document.Add(details);
            document.Add(new iText.Paragraph(" "));

            // Items Table
            var itemsTable = new iTextPdf.PdfPTable(4) { WidthPercentage = 100f };
            itemsTable.SetWidths(new float[] { 3f, 1f, 1.5f, 1.5f });
            itemsTable.HeaderRows = 1;

            // Set default cell properties
            itemsTable.DefaultCell.Border = PdfPCell.LEFT_BORDER | PdfPCell.TOP_BORDER | PdfPCell.RIGHT_BORDER;
            itemsTable.DefaultCell.BorderColor = LightGray;
            itemsTable.DefaultCell.Padding = 10;

            AddCell(itemsTable, "Item Name", true);
            AddCell(itemsTable, "Quantity", true);
            AddCell(itemsTable, "Original Price", true);
            AddCell(itemsTable, "Price (USD)", true);

            decimal totalAmount = 0m;
            foreach (var item in items)
            {
                var priceInUsd = await ConvertToUsd(item.Price, item.Currency);
                AddCell(itemsTable, item.Name);
                AddCell(itemsTable, "1");
                AddCell(itemsTable, $"{item.Price:N2} {item.Currency}");
                AddCell(itemsTable, $"{priceInUsd:N2}");
                totalAmount += priceInUsd;
            }

            // Total Row (spans last two columns, left-aligned)
            var totalCell = new iTextPdf.PdfPCell(new Phrase("Total Amount (USD):",
                iText.FontFactory.GetFont(iText.FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE)))
            {
                Colspan = 3,
                BackgroundColor = BrandColor,
                HorizontalAlignment = iText.Element.ALIGN_LEFT,
                Padding = 10
            };
            itemsTable.AddCell(totalCell);

            var amountCell = new iTextPdf.PdfPCell(new Phrase(
                $"{totalAmount:N2}",
                iText.FontFactory.GetFont(iText.FontFactory.HELVETICA_BOLD, 12)))
            {
                BackgroundColor = BrandColor,
                HorizontalAlignment = iText.Element.ALIGN_RIGHT,
                Padding = 10
            };
            itemsTable.AddCell(amountCell);

            document.Add(itemsTable);

            // QR Code Section
            var request = _httpContextAccessor.HttpContext.Request;
            var protocol = request.Scheme;
            var host = request.Host;
            var url = $"{protocol}://{host}/api/invoice/{invoice.Id}/export?fileType=2";

            var qrTable = new iTextPdf.PdfPTable(1)
            {
                WidthPercentage = 30,
                HorizontalAlignment = iText.Element.ALIGN_RIGHT,
                SpacingBefore = 20
            };

            var qrCaption = new iTextPdf.PdfPCell(new Paragraph("Scan to download invoice",
                iText.FontFactory.GetFont(iText.FontFactory.HELVETICA_OBLIQUE, 10, DarkGray)))
            {
                Border = PdfPCell.NO_BORDER,
                PaddingBottom = 5
            };
            qrTable.AddCell(qrCaption);

            var qrCode = new BarcodeQRCode(url, 150, 150, null);
            var qrImage = qrCode.GetImage();
            qrImage.ScalePercent(50f);

            var qrCell = new iTextPdf.PdfPCell(qrImage)
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = iText.Element.ALIGN_CENTER,
                VerticalAlignment = iText.Element.ALIGN_MIDDLE,
                Padding = 15
            };
            qrTable.AddCell(qrCell);

            document.Add(qrTable);
        }

        private void AddStatisticsPage(
            iText.Document document,
            IReadOnlyDictionary<string, object> statistics)
        {
            var titleFont = iText.FontFactory.GetFont(
                iText.FontFactory.HELVETICA_BOLD, 16, DarkGray);
            var title = new iText.Paragraph("Statistics", titleFont)
            {
                Alignment = iText.Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            var tbl = new iTextPdf.PdfPTable(2)
            {
                WidthPercentage = 80f,
                HorizontalAlignment = iText.Element.ALIGN_CENTER
            };
            tbl.SetWidths(new float[] { 1f, 2f });

            foreach (var kv in statistics)
            {
                AddCell(tbl, kv.Key, isHeader: true);
                AddCell(tbl, kv.Value?.ToString() ?? "N/A");
            }

            document.Add(tbl);
        }

        private void AddCell(
            iTextPdf.PdfPTable table,
            string text,
            bool isHeader = false,
            int colspan = 1,
            BaseColor textColor = null)
        {
            var font = iText.FontFactory.GetFont(
                iText.FontFactory.HELVETICA,
                10,
                isHeader ? iText.Font.BOLD : iText.Font.NORMAL,
                textColor ?? BaseColor.BLACK);

            var cell = new iTextPdf.PdfPCell(new iText.Phrase(text, font))
            {
                Colspan = colspan,
                Padding = 8
            };

            if (isHeader)
            {
                cell.BackgroundColor = LightGray;
                cell.HorizontalAlignment = iText.Element.ALIGN_CENTER;
            }

            table.AddCell(cell);
        }

        private BaseColor GetStatusColor(string status)
        {
            return status switch
            {
                "Paid" => BaseColor.GREEN,
                "Overdue" => BaseColor.RED,
                _ => BaseColor.ORANGE
            };
        }
    }
}