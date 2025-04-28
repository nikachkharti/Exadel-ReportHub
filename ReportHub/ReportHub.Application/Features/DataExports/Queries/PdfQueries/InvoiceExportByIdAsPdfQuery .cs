using MediatR;
using ReportHub.Application.Features.DataExports.Queries;

namespace ReportHub.Application.Features.DataExports.Queries.PdfQueries
{
    public record InvoiceExportByIdAsPdfQuery(string InvoiceId) : ExportBaseQuery(".pdf");
}