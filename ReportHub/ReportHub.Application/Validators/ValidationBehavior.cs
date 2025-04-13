using FluentValidation;
using MediatR;
using ReportHub.Application.Validators.Exceptions;
using System.Collections.Immutable;

namespace ReportHub.Application.Validators;

class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
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
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationFailures
                        .Where(validationResult => !validationResult.IsValid)
                        .SelectMany(validationResult => validationResult.Errors)
                        .GroupBy(validationFailures => validationFailures.PropertyName)
                        .ToImmutableDictionary(
                        pairs => pairs.Key,
                        pairs => pairs.Select(x => x.ErrorMessage).ToArray());

        if (errors.Any())
        {
            throw new InputValidationException(errors);
        }

        return await next();
    }
}
