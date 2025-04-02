using MediatR;
using ReportHub.Application.Features.DataExports.Queries.CsvQueries;

namespace ReportHub.Application.Features.DataExports.Handlers.CsvHandlers;

public class InvoiceExportAsCsvQueryHandler : IRequestHandler<InvoiceExportAsCsvQuery, Stream>
{
    public Task<Stream> Handle(InvoiceExportAsCsvQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
