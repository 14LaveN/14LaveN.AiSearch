using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.ApiHelpers.Middlewares;

/// <summary>
/// Represents the request logging middleware class.
/// </summary>
/// <param name="next">The next request delegate.</param>
public sealed class IdempotentRequestMiddleware(
    RequestDelegate next)
{
    /// <summary>
    /// Invoke middleware async.
    /// </summary>
    /// <param name="context">The http context.</param>
    public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
    {
        Ulid request = Ulid.NewUlid();
        context.Request.Headers["X-Idempotency-Key"] = request.ToString();

        await next.Invoke(context);
    }
}