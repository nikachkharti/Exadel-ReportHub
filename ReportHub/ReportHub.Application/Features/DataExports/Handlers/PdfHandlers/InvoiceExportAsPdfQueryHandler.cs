using MediatR;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.DataExports.Queries.PdfQueries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.DataExports.Handlers.PdfHandlers
{
    public class InvoiceExportAsPdfQueryHandler : IRequestHandler<InvoiceExportAsPdfQuery, Stream>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPdfService _pdfService;
        private readonly IInvoiceLogRepository _invoiceLogRepository;
        private readonly IUserContextService _userContext;
        private IEnumerable<Invoice> invoices;


        public InvoiceExportAsPdfQueryHandler(
            IInvoiceRepository invoiceRepository,
            IPdfService pdfService,
            IInvoiceLogRepository invoiceLogRepository,
            IUserContextService userContext)
        {
            _invoiceRepository = invoiceRepository;
            _pdfService = pdfService;
            _invoiceLogRepository = invoiceLogRepository;
            _userContext = userContext;
        }

        public async Task<Stream> Handle(
            InvoiceExportAsPdfQuery request,
            CancellationToken cancellationToken)
        {
            var authenticatedUserId = _userContext.GetUserId();

            try
            {
                // load all invoices
                invoices = await _invoiceRepository.GetAll(cancellationToken);
                var summary = GetSummary(invoices);

                if (invoices.Any())
                {
                    foreach (var invoice in invoices)
                    {
                        await _invoiceLogRepository.Insert(new InvoiceLog()
                        {
                            UserId = authenticatedUserId,
                            InvoiceId = invoice.Id,
                            TimeStamp = DateTime.UtcNow,
                            Status = "Success",
                            IsDeleted = invoice.IsDeleted
                        }, cancellationToken);
                    }
                }

                // generate PDF stream with summary
                return await _pdfService.WriteAllAsync(invoices, summary, cancellationToken);
            }
            catch
            {

                if (invoices.Any())
                {
                    foreach (var invoice in invoices)
                    {
                        await _invoiceLogRepository.Insert(new InvoiceLog()
                        {
                            UserId = authenticatedUserId,
                            InvoiceId = invoice.Id,
                            TimeStamp = DateTime.UtcNow,
                            Status = "Failure"
                        }, cancellationToken);
                    }
                }

                throw;
            }
        }

        private IReadOnlyDictionary<string, object> GetSummary(
            IEnumerable<Invoice> invoices)
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
