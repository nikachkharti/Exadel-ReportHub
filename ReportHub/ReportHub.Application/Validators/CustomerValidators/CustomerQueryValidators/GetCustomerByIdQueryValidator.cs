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
    }
}
