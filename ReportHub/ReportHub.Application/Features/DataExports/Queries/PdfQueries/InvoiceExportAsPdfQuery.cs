using ReportHub.Application.Features.DataExports.Queries;

namespace ReportHub.Application.Features.DataExports.Queries.PdfQueries;

public record InvoiceExportAsPdfQuery() : ExportBaseQuery(".pdf");

