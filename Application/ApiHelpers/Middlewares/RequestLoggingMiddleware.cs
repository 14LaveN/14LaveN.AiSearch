using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TeamTasks.Application.ApiHelpers.Middlewares;

/// <summary>
/// Represents the request logging middleware class.
/// </summary>
/// <param name="logger">The logger.</param>
/// <param name="next">The next request delegate.</param>
public sealed class RequestLoggingMiddleware(
    ILogger logger,
    RequestDelegate next)
{
    /// <summary>
    /// Invoke middleware async. 
    /// </summary>
    /// <param name="context">The http context.</param>
    public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("Get request: {Request}", context.Request);

        await next.Invoke(context);
    }
}