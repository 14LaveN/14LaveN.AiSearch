using Microsoft.Extensions.Logging;

namespace Application.ApiHelpers.Middlewares.DelegatingHandlers;

/// <summary>
/// Represents the logging handler class.
/// </summary>
/// <param name="logger">The logger.</param>
public sealed class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Request: {request}");
        var response = await base.SendAsync(request, cancellationToken);
        
        logger.LogInformation($"Response: {response}");
        return response;
    }
}