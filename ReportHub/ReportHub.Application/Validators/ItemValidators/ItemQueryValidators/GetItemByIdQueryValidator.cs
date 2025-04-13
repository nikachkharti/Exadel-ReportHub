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
    }
}
