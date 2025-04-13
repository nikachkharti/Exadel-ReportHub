using FluentValidation;
using ReportHub.Application.Features.Invoices.Queries;

namespace ReportHub.Application.Validators.InvoiceValidators.InvoiceQueryValidators;

public class GetInvoicesByIdQueryValidator : AbstractValidator<GetInvoicesByIdQuery>
{
    public GetInvoicesByIdQueryValidator()
    {
        RuleFor(x => x.Id)
        .NotEmpty()
        .WithMessage("Id is required.");
    }
}
