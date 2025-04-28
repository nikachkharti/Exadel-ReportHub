using MediatR;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.DataExports.Queries.PdfQueries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.DataExports.Handlers.PdfHandlers;

public class InvoiceExportByIdAsPdfQueryHandler : IRequestHandler<InvoiceExportByIdAsPdfQuery, Stream>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPdfService _pdfService;

    public InvoiceExportByIdAsPdfQueryHandler(
        IInvoiceRepository invoiceRepository,
        IPdfService pdfService)
    {
        _invoiceRepository = invoiceRepository;
        _pdfService = pdfService;
    }

    public async Task<Stream> Handle(
        InvoiceExportByIdAsPdfQuery request,
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.Get(i => request.InvoiceId == i.Id);
        var stats = GetStatistics(invoice);

        return await _pdfService.WriteInvoiceAsync(
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
