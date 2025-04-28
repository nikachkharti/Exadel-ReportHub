using ReportHub.Application.Features.DataExports.Queries;

namespace ReportHub.Application.Features.DataExports.Queries.ExcelQueries;

public record InvoiceExportByIdAsExcelQuery(string InvoiceId) : ExportBaseQuery(".xlsx");
