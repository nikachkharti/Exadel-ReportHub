using MediatR;
using ReportHub.Application.Contracts;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Features.DataImports.Queries.CsvQueries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.DataImports.Handlers.CsvHandlers;

public class InvoiceImportAsCsvQueryHandler : IRequestHandler<InvoiceImportAsCsvQuery, bool>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICsvService _csvService;

    public InvoiceImportAsCsvQueryHandler(IInvoiceRepository invoiceRepository, ICsvService csvService)
    {
        _invoiceRepository = invoiceRepository;
        _csvService = csvService;
    }

    public async Task<bool> Handle(InvoiceImportAsCsvQuery request, CancellationToken cancellationToken)
    {
        var invoices = _csvService.ReadAllAsync<Invoice>(request.Stream, cancellationToken);

        var newInvoices = await GetIfNotExist(invoices, cancellationToken);

        if (newInvoices.Count == 0) return false;

        await _invoiceRepository.InsertMultiple(newInvoices, cancellationToken);

        return true;
    }

    private async Task<List<Invoice>> GetIfNotExist(IAsyncEnumerable<Invoice> invoices, CancellationToken cancellationToken)
    {
        var newInvoices = new List<Invoice>();

        await foreach (var invoice in invoices)
        {
            if (invoice is null) continue;

            var exist = await _invoiceRepository.Get(i => i.InvoiceId.Equals(invoice.InvoiceId), cancellationToken);

            if (exist is not null) continue;

            newInvoices.Add(invoice);
        }

        return newInvoices;
    }
}
