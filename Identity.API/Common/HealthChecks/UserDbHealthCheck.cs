using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Identity.API.Common.HealthChecks;

internal sealed class UserDbHealthCheck
    : HealthCheckService
{
    public override Task<HealthReport> CheckHealthAsync(
        Func<HealthCheckRegistration, bool>? predicate,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }
}