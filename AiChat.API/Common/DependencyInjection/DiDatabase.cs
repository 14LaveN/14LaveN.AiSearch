using Application.Core.Extensions;
using Database.MetricsAndRabbitMessages;
using Domain.Core.Utility;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Persistence;

namespace AiChat.API.Common.DependencyInjection;

internal static class DiDatabase
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environment">The environemnt.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));

        services
            .AddBaseDatabase(configuration)
            .AddMigration<UserDbContext, UserDbContextSeed>()
            .AddMongoDatabase(configuration);
        
        string pathToFirebaseConfig = environment.IsDevelopment() 
            ? @"G:\DotNetProjects\TeamTasks\firebase.json" 
            : "/app/firebase.json";

        services
            .AddHealthChecks()
            .AddRedis(
            configuration.GetConnectionStringOrThrow("Redis"),
            name: "redis",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "db", "redis" });

        //TODO FirebaseApp.Create(new AppOptions
        //TODO {
        //TODO     Credential = GoogleCredential.FromFile(pathToFirebaseConfig),
        //TODO });
        
        return services;
    }
}