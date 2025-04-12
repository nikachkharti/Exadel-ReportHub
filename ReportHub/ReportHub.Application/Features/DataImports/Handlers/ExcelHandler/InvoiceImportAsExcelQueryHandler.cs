using MediatR;
using ReportHub.Application.Contracts;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Features.DataImports.Queries.ExcelQueries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.DataImports.Handlers.ExcelHandlers;

public class InvoiceImportAsExcelQueryHandler : IRequestHandler<InvoiceImportAsExcelQuery, bool>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IExcelService _excelService;

    public InvoiceImportAsExcelQueryHandler(IInvoiceRepository invoiceRepository, IExcelService excelService)
    {
        _invoiceRepository = invoiceRepository;
        _excelService = excelService;
    }

    public async Task<bool> Handle(InvoiceImportAsExcelQuery request, CancellationToken cancellationToken)
    {
        var invoices = _excelService.ReadAllAsync<Invoice>(request.Stream, cancellationToken);

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

            var exist = await _invoiceRepository.Get(i => i.Id.Equals(invoice.Id), cancellationToken);

            if (exist is not null) continue;

            newInvoices.Add(invoice);
        }

        return newInvoices;
    }
}
