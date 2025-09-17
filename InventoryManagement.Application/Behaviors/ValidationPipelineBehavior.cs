using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Behaviors;

/// <summary>
/// Pipeline behavior for request validation
/// </summary>
public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationPipelineBehavior<TRequest, TResponse>> _logger;

    public ValidationPipelineBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationPipelineBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                _logger.LogWarning("Validation failed for request {RequestType}. Errors: {ValidationErrors}",
                    typeof(TRequest).Name,
                    string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}")));

                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
