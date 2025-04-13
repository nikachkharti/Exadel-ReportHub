using FluentValidation;
using ReportHub.Application.Features.Customers.Commands;

namespace ReportHub.Application.Validators.CustomerValidators.CustomerCommandValidators;

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}
