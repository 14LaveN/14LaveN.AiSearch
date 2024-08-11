using MediatR.NotificationPublishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using RabbitMq.Abstractions;
using RabbitMq.Abstractions.Settings;
using RabbitMq.Messaging;

namespace RabbitMq;

public static class DependencyInjection
{
    public static IEventBusBuilder AddRabbitMq(this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Program>();
            
            x.NotificationPublisher = new TaskWhenAllPublisher();
            x.NotificationPublisherType = typeof(TaskWhenAllPublisher);
        });
        
        ArgumentNullException.ThrowIfNull(services);
        
        // RabbitMQ.Client doesn't have built-in support for OpenTelemetry, so we need to add it ourselves
        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddSource(RabbitMqTelemetry.ActivitySourceName);
            });
        
        services.AddSingleton<RabbitMqTelemetry>();
        services.AddSingleton<IIntegrationEventPublisher, IntegrationEventPublisher>();
        
        // Start consuming messages as soon as the application starts
        services.AddSingleton<IHostedService>(sp => 
            sp.GetRequiredService<IntegrationEventConsumerBackgroundService>());
        
        services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SettingsKey));
        
        services.AddOptions<MessageBrokerSettings>()
            .BindConfiguration(MessageBrokerSettings.SettingsKey)
            .ValidateOnStart();
        
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();
        
        return new EventBusBuilder(services);; 
    }
    
    private class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
    {
        public IServiceCollection Services => services;
    }
}