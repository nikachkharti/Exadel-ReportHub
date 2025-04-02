using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.DataImports.Queries.CsvQueries;

public record InvoiceImportAsCsvQuery(Stream Stream, string FileExtension) : ImportBaseQuery<Invoice>(Stream,FileExtension);
