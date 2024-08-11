using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Domain.Core.Primitives.Result;

namespace Application.Core.Behaviours;

/// <summary>
/// Represents the request logging pipeline behavior class.
/// </summary>
/// <param name="logger">The logger.</param>
/// <typeparam name="TRequest">The generic request class.</typeparam>
/// <typeparam name="TResponse">The generic response type.</typeparam>
public sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        logger.LogInformation(
            $"Processing request {requestName}");

        TResponse result = await next();

        if (result.IsSuccess)
            logger.LogInformation(
                $"Completed request {requestName}");
        else
            using (LogContext.PushProperty("Error", result.Error, true))
            {
                logger.LogError(
                    $"Completed request {requestName} with error");
            }
        
        return result;
    }
}