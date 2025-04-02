using ReportHub.Application.Features.DataImports.Queries.CsvQueries;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Validators.DataImportQueryValidators.CsvQueryValidators;

public class InvoiceImportAsCsvQueryValidator : ImportQueryAsCsvQueryBaseValidator<InvoiceImportAsCsvQuery, Invoice>{}
