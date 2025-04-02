using FluentValidation;
using ReportHub.Application.Features.DataImports.Queries;

namespace ReportHub.Application.Validators.DataImportQueryValidators.CsvQueryValidators;

public class ImportQueryAsCsvQueryBaseValidator<TQuery, TType> : AbstractValidator<TQuery> 
    where TQuery : ImportBaseQuery<TType> where TType : class
{
    public ImportQueryAsCsvQueryBaseValidator()
    {
        RuleFor(x => x.Stream)
            .NotNull()
            .WithMessage("File is required");

        RuleFor(x => x.FileExtension)
            .Equal(".csv")
            .WithMessage("File extension is required");
    }
}
