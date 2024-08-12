using MediatR.NotificationPublishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
        ArgumentNullException.ThrowIfNull(services);
        
        services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SettingsKey));
        
        services.AddOptions<MessageBrokerSettings>()
            .BindConfiguration(MessageBrokerSettings.SettingsKey)
            .ValidateOnStart();

        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Program>();
            
            x.NotificationPublisher = new TaskWhenAllPublisher();
            x.NotificationPublisherType = typeof(TaskWhenAllPublisher);
        });
        
        services
            .AddSingleton<RabbitMqTelemetry>()
            .AddSingleton<IIntegrationEventPublisher, IntegrationEventPublisher>();

        services
            .AddSingleton<IConnectionFactory>(sp =>
            new ConnectionFactory { Uri = new Uri(MessageBrokerSettings.AmqpLink) })
            .AddSingleton<IConnection>(sp =>
            {
                IConnectionFactory factory = sp.GetRequiredService<IConnectionFactory>();
                return factory.CreateConnection();
            })
            .AddSingleton<IModel>(sp =>
            {
                IConnection connection = sp.GetRequiredService<IConnection>();
                return connection.CreateModel();
            });
        
        services.AddHostedService<IntegrationEventConsumerBackgroundService>();
        
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();
        
        // RabbitMQ.Client doesn't have built-in support for OpenTelemetry, so we need to add it ourselves
        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddSource(RabbitMqTelemetry.ActivitySourceName);
            });
        
        return new EventBusBuilder(services);
    }
    
    private class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
    {
        public IServiceCollection Services => services;
    }
}