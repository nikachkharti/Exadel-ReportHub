using FluentValidation;
using MediatR;

namespace ReportHub.Identity.Application.Validators;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        foreach(var validator in _validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }

        return await next();
    }
}
