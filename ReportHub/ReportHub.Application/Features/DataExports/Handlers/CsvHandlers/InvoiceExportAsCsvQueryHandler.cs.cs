using MediatR;
using ReportHub.Application.Contracts;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Features.DataExports.Queries.CsvQueries;

namespace ReportHub.Application.Features.DataExports.Handlers.CsvHandlers;

public class InvoiceExportAsCsvQueryHandler : IRequestHandler<InvoiceExportAsCsvQuery, Stream>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICsvService _csvService;   
    public InvoiceExportAsCsvQueryHandler(IInvoiceRepository invoiceRepository, ICsvService csvService)
    {
        _invoiceRepository = invoiceRepository;
        _csvService = csvService;
    }
    public async Task<Stream> Handle(InvoiceExportAsCsvQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetAll(cancellationToken);
        
        return await _csvService.WriteAllAsync(invoices, GetSummary(invoices), cancellationToken);
    }

    private IReadOnlyDictionary<string, object> GetSummary(IEnumerable<Domain.Entities.Invoice> invoices)
    {
        return new Dictionary<string, object>
        {
            { "TotalInvoices", invoices.Count() },
            { "Paid" , invoices.Where(x => x.PaymentStatus.Equals("Paid")).Count() },
            { "Pending", invoices.Where(x => x.PaymentStatus.Equals("Pending")).Count() },
            { "Overdue", invoices.Where(x => x.PaymentStatus.Equals("Overdue")).Count() }
        };
    }
}
