using MediatR.NotificationPublishers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Persistence;
using Persistence.Infrastructure;
using TeamTasks.Application.Core.Abstractions;
using TeamTasks.Application.Core.Abstractions.Idempotency;
using TeamTasks.Persistence;
using TeamTasks.Persistence.Idempotency;

namespace TeamTasks.Persistence;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddBaseDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var connectionString = configuration.GetConnectionString(ConnectionString.SettingsKey);
        
        if (connectionString is not null)
            services.AddHealthChecks()
                .AddNpgSql(connectionString);
        
        services.AddDbContext<BaseDbContext>((sp, o) =>
            o.UseNpgsql(connectionString, act
                    =>
            {
                act.EnableRetryOnFailure(3);
                act.CommandTimeout(30);
                act.MigrationsAssembly("Persistence");
                act.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            })
                .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.ForeignKeyPropertiesMappedToUnrelatedTables))
                .LogTo(Console.WriteLine)
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .EnableServiceProviderCaching()
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDbContext, BaseDbContext>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();
        
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Program>();

            x.NotificationPublisher = new TaskWhenAllPublisher();
            x.NotificationPublisherType = typeof(TaskWhenAllPublisher);
        });
        
        return services;
    }
}