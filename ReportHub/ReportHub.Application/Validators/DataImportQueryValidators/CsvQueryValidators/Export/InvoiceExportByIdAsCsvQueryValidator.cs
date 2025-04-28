using FluentValidation;
using MongoDB.Bson;
using ReportHub.Application.Features.DataExports.Queries.CsvQueries;

namespace ReportHub.Application.Validators.DataImportQueryValidators.CsvQueryValidators.Export;

public class InvoiceExportByIdAsCsvQueryValidator : AbstractValidator<InvoiceExportByIdAsCsvQuery>
{
    public InvoiceExportByIdAsCsvQueryValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty()
              .WithMessage("InvoiceId is required.")
              .Must(x => ObjectId.TryParse(x, out var id))
              .WithMessage("InvoiceId must be a valid 24-character hex string.");
    }
}
