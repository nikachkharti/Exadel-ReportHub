using MediatR;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.DataExports.Queries.ExcelQueries;
using ReportHub.Application.Features.DataExports.Queries.PdfQueries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.DataExports.Handlers.ExcelHandlers;

public class InvoiceExportByIdAsExcelQueryHandler : IRequestHandler<InvoiceExportByIdAsExcelQuery, Stream>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IExcelService _excelService;

    public InvoiceExportByIdAsExcelQueryHandler(
        IInvoiceRepository invoiceRepository,
        IExcelService excelService)
    {
        _invoiceRepository = invoiceRepository;
        _excelService = excelService;
    }

    public async Task<Stream> Handle(
        InvoiceExportByIdAsExcelQuery request,
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.Get(i => request.InvoiceId == i.Id);
        var stats = GetStatistics(invoice);

        return await _excelService.WriteInvoiceAsync(
            invoice,
            stats,
            cancellationToken);
    }

    private static IReadOnlyDictionary<string, object> GetStatistics(Invoice invoice)
    {
        return new Dictionary<string, object>
        {
        };
    }
}
