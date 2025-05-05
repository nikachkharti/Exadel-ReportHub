using FluentValidation;
using ReportHub.Application.Features.Invoices.Queries;

namespace ReportHub.Application.Validators.InvoiceValidators.InvoiceQueryValidators;

public class GetStatisticsQueryValidator : AbstractValidator<GetStatistcsQuery>
{
    public GetStatisticsQueryValidator()
    {
        RuleFor(x => x.PaymentStatus)
            .NotEmpty()
            .WithMessage("Payment status is required");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be in 3 length code");
    }
}
