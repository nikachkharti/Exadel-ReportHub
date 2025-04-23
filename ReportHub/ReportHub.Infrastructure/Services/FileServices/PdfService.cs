using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// alias iTextSharp namespaces to avoid collisions
using iText = iTextSharp.text;
using iTextPdf = iTextSharp.text.pdf;

using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;

namespace ReportHub.Infrastructure.Services.FileServices
{
    internal class PdfService : IPdfService
    {
        private readonly IClientRepository _clientRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IItemRepository _itemRepo;

        public PdfService(
            IClientRepository clientRepo,
            ICustomerRepository customerRepo,
            IItemRepository itemRepo)
        {
            _clientRepo = clientRepo;
            _customerRepo = customerRepo;
            _itemRepo = itemRepo;
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
            // fetch related data
            var client = await _clientRepo.Get(c => c.Id == invoice.ClientId);
            var customer = await _customerRepo.Get(c => c.Id == invoice.CustomerId);
            var items = await _itemRepo.GetAll(i => invoice.ItemIds.Contains(i.Id));

            // Title header
            var titleFont = iText.FontFactory.GetFont(
                iText.FontFactory.HELVETICA_BOLD, 18);
            var title = new iText.Paragraph(
                $"Invoice Number: {invoice.Id}", titleFont)
            {
                Alignment = iText.Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            // Details table
            var details = new iTextPdf.PdfPTable(2) { WidthPercentage = 100f };
            details.SetWidths(new float[] { 1f, 2f });

            AddCell(details, "Customer:", true);
            AddCell(details, customer?.Name ?? "N/A");

            AddCell(details, "Client:", true);
            AddCell(details, client?.Name ?? "N/A");

            AddCell(details, "Issue Date:", true);
            AddCell(details, invoice.IssueDate
                                     .ToString("dd-MM-yyyy"));

            AddCell(details, "Due Date:", true);
            AddCell(details, invoice.DueDate
                                     .ToString("dd-MM-yyyy"));

            AddCell(details, "Payment Status:", true);
            AddCell(details, invoice.PaymentStatus);

            //AddCell(details, "Bank Account:", true);
            //AddCell(details, client?.BankAccountNumber ?? "N/A");

            document.Add(details);
            document.Add(new iText.Paragraph(" "));

            // Items table
            var itemsTable = new iTextPdf.PdfPTable(4) { WidthPercentage = 100f };
            itemsTable.SetWidths(new float[] { 2f, 1f, 1f, 1f });

            AddCell(itemsTable, "Item Name", true);
            AddCell(itemsTable, "Quantity", true);
            AddCell(itemsTable, "Price", true);
            AddCell(itemsTable, "Total", true);

            decimal totalAmount = 0m;
            foreach (var item in items)
            {
                AddCell(itemsTable, item.Name);
                AddCell(itemsTable, "1");  

                AddCell(itemsTable,
                    $"{item.Price.ToString("N2", CultureInfo.InvariantCulture)} {invoice.Currency}");

                var lineTotal = item.Price; 
                totalAmount += lineTotal;

                AddCell(itemsTable,
                    $"{lineTotal.ToString("N2", CultureInfo.InvariantCulture)} {invoice.Currency}");
            }

            // Grand total row
            AddCell(itemsTable, "Total Amount:", true, colspan: 3);
            AddCell(itemsTable,
                $"{totalAmount.ToString("N2", CultureInfo.InvariantCulture)} {invoice.Currency}",
                isHeader: true);

            document.Add(itemsTable);
        }

        private void AddStatisticsPage(
            iText.Document document,
            IReadOnlyDictionary<string, object> statistics)
        {
            var titleFont = iText.FontFactory.GetFont(
                iText.FontFactory.HELVETICA_BOLD, 16);
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
            int colspan = 1)
        {
            var phrase = new iText.Phrase(text);
            var cell = new iTextPdf.PdfPCell(phrase)
            {
                Colspan = colspan,
                Padding = 5
            };

            if (isHeader)
            {
                cell.BackgroundColor = iText.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = iText.Element.ALIGN_CENTER;
            }

            table.AddCell(cell);
        }
    }
}
