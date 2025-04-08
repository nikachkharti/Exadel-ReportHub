namespace ReportHub.Application.Features.DataImports.Queries.ExcelQueries;

public record InvoiceImportAsExcelQuery(Stream Stream, string FileExtension) : ImportBaseQuery(Stream, FileExtension);
