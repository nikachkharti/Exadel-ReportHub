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

        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("Country is required.");
    }
}
