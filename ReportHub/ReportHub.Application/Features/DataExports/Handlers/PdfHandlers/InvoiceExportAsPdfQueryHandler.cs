using MediatR;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.DataExports.Queries.PdfQueries;

namespace ReportHub.Application.Features.DataExports.Handlers.PdfHandlers
{
    public class InvoiceExportAsPdfQueryHandler : IRequestHandler<InvoiceExportAsPdfQuery, Stream>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPdfService _pdfService;

        public InvoiceExportAsPdfQueryHandler(
            IInvoiceRepository invoiceRepository,
            IPdfService pdfService)
        {
            _invoiceRepository = invoiceRepository;
            _pdfService = pdfService;
        }

        public async Task<Stream> Handle(
            InvoiceExportAsPdfQuery request,
            CancellationToken cancellationToken)
        {
            // load all invoices
            var invoices = await _invoiceRepository
                .GetAll(cancellationToken);

            // generate PDF stream with summary
            return await _pdfService.WriteAllAsync(
                invoices,
                GetSummary(invoices),
                cancellationToken);
        }

        private IReadOnlyDictionary<string, object> GetSummary(
            IEnumerable<Domain.Entities.Invoice> invoices)
        {
            return new Dictionary<string, object>
            {
                { "Total Invoices", invoices.Count() },
                { "Paid",           invoices.Count(x => x.PaymentStatus == "Paid") },
                { "Pending",        invoices.Count(x => x.PaymentStatus == "Pending") },
                { "Overdue",        invoices.Count(x => x.PaymentStatus == "Overdue") }
            };
        }
    }
}
