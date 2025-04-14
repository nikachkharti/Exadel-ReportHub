using FluentValidation;
using ReportHub.Application.Features.Customers.Commands;

namespace ReportHub.Application.Validators.CustomerValidators.CustomerCommandValidators;

public class UpdateCustomerCommandValidator : BaseCustomerCommandValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}
