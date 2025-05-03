using MediatR;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.DataExports.Queries.CsvQueries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.DataExports.Handlers.CsvHandlers;

public class InvoiceExportAsCsvQueryHandler : IRequestHandler<InvoiceExportAsCsvQuery, Stream>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IInvoiceLogRepository _invoiceLogRepository;
    private readonly IUserContextService _userContext;
    private readonly ICsvService _csvService;
    private IEnumerable<Invoice> invoices;
    public InvoiceExportAsCsvQueryHandler(
            IInvoiceRepository invoiceRepository,
            ICsvService csvService,
            IInvoiceLogRepository invoiceLogRepository,
            IUserContextService userContext)
    {
        _invoiceRepository = invoiceRepository;
        _invoiceLogRepository = invoiceLogRepository;
        _csvService = csvService;
        _userContext = userContext;
    }
    public async Task<Stream> Handle(InvoiceExportAsCsvQuery request, CancellationToken cancellationToken)
    {
        var authenticatedUserId = _userContext.GetUserId();

        try
        {
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

            return await _csvService.WriteAllAsync(invoices, summary, cancellationToken);
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

    private IReadOnlyDictionary<string, object> GetSummary(IEnumerable<Invoice> invoices)
    {
        return new Dictionary<string, object>
        {
            { "All Statistics", ""},
            { "TotalInvoices", invoices.Count() },
            { "Paid" , invoices.Where(x => x.PaymentStatus.Equals("Paid")).Count() },
            { "Pending", invoices.Where(x => x.PaymentStatus.Equals("Pending")).Count() },
            { "Overdue", invoices.Where(x => x.PaymentStatus.Equals("Overdue")).Count() }
        };
    }
}
