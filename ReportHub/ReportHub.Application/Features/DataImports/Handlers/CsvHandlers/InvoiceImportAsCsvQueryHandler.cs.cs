using MediatR;
using ReportHub.Application.Features.DataImports.Queries.CsvQueries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.DataImports.Handlers.CsvHandlers;

public class InvoiceImportAsCsvQueryHandler : IRequestHandler<InvoiceImportAsCsvQuery, IEnumerable<Invoice>>
{
    public Task<IEnumerable<Invoice>> Handle(InvoiceImportAsCsvQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
