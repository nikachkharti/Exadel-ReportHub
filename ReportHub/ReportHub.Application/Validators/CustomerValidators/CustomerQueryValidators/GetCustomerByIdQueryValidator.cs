using FluentValidation;
using ReportHub.Application.Features.Customers.Queries;

namespace ReportHub.Application.Validators.CustomerValidators.CustomerQueryValidators;

public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(x => x.Id)
          .NotEmpty()
          .WithMessage("Id is required.");

        RuleFor(x => x.Id)
            .Length(24)
            .WithMessage("Id is not a valid 24 digit hex string.");
    }
}
