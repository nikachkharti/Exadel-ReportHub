using FluentValidation;
using MongoDB.Bson;
using ReportHub.Application.Features.DataExports.Queries.PdfQueries;

namespace ReportHub.Application.Validators.DataImportQueryValidators.PdfQueryValidators.Export;

public class InvoiceExportByIdAsPdfQueryValidator : AbstractValidator<InvoiceExportByIdAsPdfQuery>
{
    public InvoiceExportByIdAsPdfQueryValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty()
              .WithMessage("InvoiceId is required.")
              .Must(x => ObjectId.TryParse(x, out var id))
              .WithMessage("InvoiceId must be a valid 24-character hex string.");
    }
}
