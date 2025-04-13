using FluentValidation;
using ReportHub.Application.Features.Customers.Commands;

namespace ReportHub.Application.Validators.CustomerValidators.CustomerCommandValidators;

public class BaseCustomerCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : BaseCustomerCommand
{
    public BaseCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");
    }
}
