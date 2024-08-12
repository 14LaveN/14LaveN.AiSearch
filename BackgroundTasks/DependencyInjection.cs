using System.Reflection;
using BackgroundTasks.Services;
using BackgroundTasks.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BackgroundTasks.Services;
using BackgroundTasks.Settings;
using BackgroundTasks.Tasks;

namespace BackgroundTasks;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The same service collection.</returns>
    [Obsolete("Obsolete")]
    public static IServiceCollection AddBackgroundTasks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(x=>
            x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.Configure<BackgroundTaskSettings>(configuration.GetSection(BackgroundTaskSettings.SettingsKey));

        services
            .AddOptions<BackgroundTaskSettings>()
            .BindConfiguration(BackgroundTaskSettings.SettingsKey)
            .ValidateOnStart();
        
        services
            .AddHostedService<SaveMetricsBackgroundService>()
            .AddHostedService<CreateReportBackgroundService>();
        
        services
            .AddScoped<IIntegrationEventConsumer, IntegrationEventConsumer>()
            .AddScoped<ICreateReportProducer, CreateReportProducer>();

        services.Configure<HostOptions>(options =>
        {
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = true;
        });
        
        return services;
    }
}