namespace ReportHub.Application.Features.DataExports.Queries.CsvQueries;

public record InvoiceExportByIdAsCsvQuery(string InvoiceId) : ExportBaseQuery(".csv");
