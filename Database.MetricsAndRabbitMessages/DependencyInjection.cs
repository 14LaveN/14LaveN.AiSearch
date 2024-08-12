using Database.MetricsAndRabbitMessages.Data.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Core.Settings;
using Database.MetricsAndRabbitMessages;
using Database.MetricsAndRabbitMessages.Data.Interfaces;
using Database.MetricsAndRabbitMessages.Data.Repositories;
using Domain.Entities;

namespace Database.MetricsAndRabbitMessages;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddMongoDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        
        services.Configure<MongoSettings>(configuration.GetSection(MongoSettings.MongoSettingsKey));
        
        services.AddOptions<MongoSettings>()
            .BindConfiguration(MongoSettings.MongoSettingsKey)
            .ValidateOnStart();

        services.AddSingleton<IMetricsRepository, MetricsRepository>();
        services.AddSingleton<IMongoRepository<RabbitMessage>, RabbitMessagesRepository>();
        services.AddSingleton<ICommonMongoDbContext, CommonMongoDbContext>();
        
        services.AddTransient<MongoSettings>();
        
        //TODO services.AddHealthChecks()
        //TODO     .AddMongoDb(configuration.GetConnectionString("MongoConnection")!);

        return services;
    }
}