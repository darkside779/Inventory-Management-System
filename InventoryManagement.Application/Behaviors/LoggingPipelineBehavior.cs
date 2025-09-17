using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace InventoryManagement.Application.Behaviors;

/// <summary>
/// Pipeline behavior for request logging and performance monitoring
/// </summary>
public class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestGuid = Guid.NewGuid().ToString();

        _logger.LogInformation("[{RequestId}] Starting request: {RequestName}", requestGuid, requestName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation("[{RequestId}] Completed request: {RequestName} in {ElapsedMilliseconds}ms", 
                requestGuid, requestName, stopwatch.ElapsedMilliseconds);

            if (stopwatch.ElapsedMilliseconds > 5000) // Log warning for long-running requests
            {
                _logger.LogWarning("[{RequestId}] Long running request: {RequestName} took {ElapsedMilliseconds}ms", 
                    requestGuid, requestName, stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex, "[{RequestId}] Request failed: {RequestName} after {ElapsedMilliseconds}ms. Error: {ErrorMessage}", 
                requestGuid, requestName, stopwatch.ElapsedMilliseconds, ex.Message);

            throw;
        }
    }
}
