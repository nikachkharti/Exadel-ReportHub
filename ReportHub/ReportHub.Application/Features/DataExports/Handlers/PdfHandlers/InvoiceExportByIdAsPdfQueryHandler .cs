using MediatR;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.DataExports.Queries.PdfQueries;
using ReportHub.Application.Validators.Exceptions;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.DataExports.Handlers.PdfHandlers;

public class InvoiceExportByIdAsPdfQueryHandler : BaseFeature, IRequestHandler<InvoiceExportByIdAsPdfQuery, Stream>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPdfService _pdfService;
    public InvoiceExportByIdAsPdfQueryHandler(
        IInvoiceRepository invoiceRepository,
        IPdfService pdfService, IRequestContextService requestContext) : base(requestContext)
    {
        _invoiceRepository = invoiceRepository;
        _pdfService = pdfService;
    }

    public async Task<Stream> Handle(
        InvoiceExportByIdAsPdfQuery request,
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.Get(i => request.InvoiceId == i.Id);

        if (invoice is null)
        {
            throw new NotFoundException($"Invoice with id {request.InvoiceId} not found");
        }

        EnsureUserHasRoleForThisClient(invoice.ClientId);

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
