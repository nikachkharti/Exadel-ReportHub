using MediatR;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.DataExports.Queries.CsvQueries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.DataExports.Handlers.CsvHandlers;

public class InvoiceExportByIdAsCsvQueryHandler : IRequestHandler<InvoiceExportByIdAsCsvQuery, Stream>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICsvService _csvService;

    public InvoiceExportByIdAsCsvQueryHandler(
        IInvoiceRepository invoiceRepository,
        ICsvService csvService)
    {
        _invoiceRepository = invoiceRepository;
        _csvService = csvService;
    }

    public async Task<Stream> Handle(
        InvoiceExportByIdAsCsvQuery request,
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.Get(i => request.InvoiceId == i.Id);
        var stats = GetStatistics(invoice);

        return await _csvService.WriteInvoiceAsync(
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
