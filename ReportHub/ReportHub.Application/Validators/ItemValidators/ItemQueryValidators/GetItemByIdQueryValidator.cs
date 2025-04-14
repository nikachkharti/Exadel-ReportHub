using FluentValidation;
using ReportHub.Application.Features.Item.Queries;

namespace ReportHub.Application.Validators.ItemValidators.ItemQueryValidators;

public class GetItemByIdQueryValidator : AbstractValidator<GetItemByIdQuery>
{
    public GetItemByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Id)
            .Length(24)
            .WithMessage("Id is not a valid 24 digit hex string.");
    }
}
