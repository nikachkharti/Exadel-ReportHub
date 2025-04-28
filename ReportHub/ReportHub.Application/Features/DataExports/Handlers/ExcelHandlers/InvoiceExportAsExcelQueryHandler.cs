using MediatR;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.DataExports.Queries.ExcelQueries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.DataExports.Handlers.ExcelHandlers;

public class InvoiceExportAsExcelQueryHandler : IRequestHandler<InvoiceExportAsExcelQuery, Stream>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IExcelService _excelService;
    private readonly IInvoiceLogRepository _invoiceLogRepository;
    private readonly IUserContextService _userContext;
    private IEnumerable<Invoice> invoices;

    public InvoiceExportAsExcelQueryHandler(
        IInvoiceRepository invoiceRepository,
        IInvoiceLogRepository invoiceLogRepository,
        IUserContextService userContextService,
        IExcelService excelService)
    {
        _invoiceRepository = invoiceRepository;
        _excelService = excelService;
        _userContext = userContextService;
        _invoiceLogRepository = invoiceLogRepository;
    }

    public async Task<Stream> Handle(InvoiceExportAsExcelQuery request, CancellationToken cancellationToken)
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

            return await _excelService.WriteAllAsync(invoices, summary, cancellationToken);
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
