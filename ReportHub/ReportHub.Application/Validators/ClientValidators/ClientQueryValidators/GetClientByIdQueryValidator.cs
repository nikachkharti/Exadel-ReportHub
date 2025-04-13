using FluentValidation;
using ReportHub.Application.Features.Clients.Queries;

namespace ReportHub.Application.Validators.ClientValidators.ClientQueryValidators;

public class GetClientByIdQueryValidator : AbstractValidator<GetClientByIdQuery>
{
    public GetClientByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
        
        RuleFor(x => x.Id)
            .Length(24)
            .WithMessage("Id is not a valid 24 digit hex string.");
    }
}
