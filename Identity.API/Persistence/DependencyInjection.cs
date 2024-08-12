using Application.Core.Abstractions;
using Application.Core.Abstractions.Idempotency;
using Identity.API.Persistence.Repositories;
using Identity.Domain.Repositories;
using MediatR.NotificationPublishers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Persistence;
using Persistence.Infrastructure;

namespace Identity.API.Persistence;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddUserDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        
        var connectionString = configuration.GetConnectionString(ConnectionString.SettingsKey);
        
        //TODO HealthChecks.
        if (connectionString is not null)
            services.AddHealthChecks()
                .AddNpgSql(connectionString);
        
        services.AddDbContext<BaseDbContext>((sp, o) =>
            o.UseNpgsql(connectionString, act
                    =>
                {
                    act.EnableRetryOnFailure(3);
                    act.CommandTimeout(30);
                    act.MigrationsAssembly("Identity.API");
                    act.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                })
                .ConfigureWarnings(warnings =>
                    warnings.Ignore(RelationalEventId.ForeignKeyPropertiesMappedToUnrelatedTables))
                .LogTo(Console.WriteLine)
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .EnableServiceProviderCaching()
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());

        services.AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IUserRepository, UserRepository>();
        
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Program>();

            x.NotificationPublisher = new TaskWhenAllPublisher();
            x.NotificationPublisherType = typeof(TaskWhenAllPublisher);
        });
        
        return services;
    }
}