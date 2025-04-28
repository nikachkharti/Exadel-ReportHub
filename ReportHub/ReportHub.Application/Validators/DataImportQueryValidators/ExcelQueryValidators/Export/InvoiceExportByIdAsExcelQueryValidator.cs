using FluentValidation;
using MongoDB.Bson;
using ReportHub.Application.Features.DataExports.Queries.ExcelQueries;

namespace ReportHub.Application.Validators.DataImportQueryValidators.ExcelQueryValidators.Export;

public class InvoiceExportByIdAsExcelQueryValidator : AbstractValidator<InvoiceExportByIdAsExcelQuery>
{
    public InvoiceExportByIdAsExcelQueryValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty()
              .WithMessage("InvoiceId is required.")
              .Must(x => ObjectId.TryParse(x, out var id))
              .WithMessage("InvoiceId must be a valid 24-character hex string.");
    }
}
