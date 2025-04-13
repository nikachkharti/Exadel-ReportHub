using MediatR;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.DataExports.Queries.ExcelQueries;

namespace ReportHub.Application.Features.DataExports.Handlers.ExcelHandlers;

public class InvoiceExportAsExcelQueryHandler : IRequestHandler<InvoiceExportAsExcelQuery, Stream>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IExcelService _excelService;

    public InvoiceExportAsExcelQueryHandler(IInvoiceRepository invoiceRepository, IExcelService excelService)
    {
        _invoiceRepository = invoiceRepository;
        _excelService = excelService;
    }

    public async Task<Stream> Handle(InvoiceExportAsExcelQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetAll(cancellationToken);

        return await _excelService.WriteAllAsync(invoices, GetSummary(invoices), cancellationToken);
    }

    private IReadOnlyDictionary<string, object> GetSummary(IEnumerable<Domain.Entities.Invoice> invoices)
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
