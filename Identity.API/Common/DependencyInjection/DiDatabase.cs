using Application.Core.Extensions;
using Domain.Core.Utility;
using Persistence;
using Database.MetricsAndRabbitMessages;
using Identity.API.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Identity.Api.Common.DependencyInjection;

internal static class DiDatabase
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));

        services
            .AddBaseDatabase(configuration)
            .AddMongoDatabase(configuration)
            .AddUserDatabase(configuration)
            .AddMigration<UserDbContext, UserDbContextSeed>();
        
        services
            .AddHealthChecks()
            .AddRedis(
            configuration.GetConnectionStringOrThrow("Redis"),
            name: "redis",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "db", "redis" });
        
        return services;
    }
}