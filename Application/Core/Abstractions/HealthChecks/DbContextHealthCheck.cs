using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Application.Core.Abstractions.HealthChecks;

public sealed class DbContextHealthCheck<TContext>(TContext dbContext) : IHealthCheck
    where TContext : IDbContext
{
    private readonly TContext _dbContext = dbContext;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Проверка наличия подключения к базе данных
            await _dbContext.ExecuteSqlAsync(
                "SELECT 1",
                Enumerable.Empty<NpgsqlParameter>(),
                cancellationToken);
            return HealthCheckResult.Healthy("Database connection is healthy.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection is unhealthy.", ex);
        }
    }
}