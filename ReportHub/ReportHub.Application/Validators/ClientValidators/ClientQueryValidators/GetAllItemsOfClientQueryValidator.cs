using FluentValidation;
using ReportHub.Application.Features.Clients.Queries;

namespace ReportHub.Application.Validators.ClientValidators.ClientQueryValidators;

public class GetAllItemsOfClientQueryValidator : AbstractValidator<GetAllItemsOfClientQuery>
{
    public GetAllItemsOfClientQueryValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}
