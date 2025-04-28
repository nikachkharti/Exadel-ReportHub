using FluentValidation;
using ReportHub.Application.Features.DataImports.Queries;

namespace ReportHub.Application.Validators.DataImportQueryValidators.CsvQueryValidators.Import;

public class ImportQueryAsCsvQueryBaseValidator<TQuery> : AbstractValidator<TQuery> 
    where TQuery : ImportBaseQuery
{
    public ImportQueryAsCsvQueryBaseValidator()
    {
        RuleFor(x => x.Stream)
            .NotNull()
            .WithMessage("File is required");

        RuleFor(x => x.FileExtension)
            .Equal(".csv")
            .WithMessage("File format should be csv. Please select correct file format");
    }
}
