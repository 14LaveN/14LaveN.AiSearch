using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Application.Core.Abstractions.Messaging;
using Database.MetricsAndRabbitMessages.Data.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMq.Abstractions;
using RabbitMq.Abstractions.Settings;
using RabbitMq.Extensions;
using ActivityExtensions = RabbitMq.Extensions.ActivityExtensions;
using ConnectionFactory = RabbitMQ.Client.ConnectionFactory;
using IConnection = RabbitMQ.Client.IConnection;

namespace RabbitMq.Messaging;

/// <summary>
/// Represents the integration event consumer background service class.
/// </summary>
internal  sealed class IntegrationEventConsumerBackgroundService(
    IServiceProvider serviceProvider,
    IModel channel,
    IConnection connection,
    ILogger<IntegrationEventConsumerBackgroundService> logger,
    IOptions<MessageBrokerSettings> messageBrokerSettingsOptions,
    IOptions<EventBusSubscriptionInfo> subscriptionOptions,
    RabbitMqTelemetry rabbitMqTelemetry)
    : IHostedService, IDisposable
{
    private readonly MessageBrokerSettings _messageBrokerSettings = messageBrokerSettingsOptions.Value;
    
    private readonly TextMapPropagator _propagator = rabbitMqTelemetry.Propagator;
    private readonly ActivitySource _activitySource = rabbitMqTelemetry.ActivitySource;
    
    private readonly EventBusSubscriptionInfo _subscriptionInfo = subscriptionOptions.Value;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Messaging is async so we don't need to wait for it to complete. On top of this
        // the APIs are blocking, so we need to run this on a background thread.
        _ =  Task.Factory.StartNew( () =>
        {
            try
            {
                logger.LogInformation("Starting RabbitMQ connection on a background thread");

                var factory = new ConnectionFactory
                {
                    Uri = new Uri(MessageBrokerSettings.AmqpLink)
                };
                connection = factory.CreateConnection();
                
                if (!connection.IsOpen)
                {
                    return;
                }

                if (logger.IsEnabled(LogLevel.Trace))
                {
                    logger.LogTrace("Creating RabbitMQ consumer channel");
                }

                channel = connection.CreateModel();

                channel.CallbackException += (sender, ea) =>
                {
                    logger.LogWarning(ea.Exception, "Error with RabbitMQ consumer channel");
                };

                channel.ExchangeDeclare(exchange: _messageBrokerSettings.QueueName + "Exchange",
                                        type: ExchangeType.Direct);

                channel.QueueDeclare(queue: _messageBrokerSettings.QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                if (logger.IsEnabled(LogLevel.Trace))
                {
                    logger.LogTrace("Starting RabbitMQ basic consume");
                }

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.Received += OnMessageReceived;

                channel.BasicConsume(
                    queue: _messageBrokerSettings.QueueName,
                    autoAck: false,
                    consumer: consumer);

                foreach (var (eventName, _) in _subscriptionInfo.EventTypes)
                {
                    channel.QueueBind(
                        queue: _messageBrokerSettings.QueueName,
                        exchange: _messageBrokerSettings.QueueName + "Exchange",
                        routingKey: eventName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error starting RabbitMQ connection");
            }
        },
        TaskCreationOptions.LongRunning);
        
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        static IEnumerable<string> ExtractTraceContextFromBasicProperties(IBasicProperties props, string key)
        {
            if (props.Headers.TryGetValue(key, out var value))
            {
                var bytes = value as byte[];
                return [Encoding.UTF8.GetString(bytes)];
            }
            return [];
        }

        // Extract the PropagationContext of the upstream parent from the message headers.
        PropagationContext parentContext = _propagator.Extract(
            default, 
            eventArgs.BasicProperties,
            ExtractTraceContextFromBasicProperties);
        
        Baggage.Current = parentContext.Baggage;

        // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
        string activityName = $"{eventArgs.RoutingKey} receive";

        using Activity? activity = _activitySource.StartActivity(
            activityName,
            ActivityKind.Client,
            parentContext.ActivityContext);

        ActivityExtensions.SetActivityContext(
            activity,
            eventArgs.RoutingKey,
            "receive");

        string? eventName = eventArgs.RoutingKey;
        string message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            activity?.SetTag("message", message);

            if (message.Contains("throw-fake-exception", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
            }

            await ProcessEvent(eventName, message);
            
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error Processing message \"{Message}\"", message);

            activity.SetExceptionTags(ex);
        }

        // Even on exception we take the message off the queue.
        // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
        // For more information see: https://www.rabbitmq.com/dlx.html
        channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);
        }

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

        if (!_subscriptionInfo.EventTypes.TryGetValue(eventName, out var eventType))
        {
            logger.LogWarning("Unable to resolve event type for event name {EventName}", eventName);
            return;
        }

        // Deserialize the event
        var integrationEvent = DeserializeMessage(message, eventType);
        
        // REVIEW: This could be done in parallel

        // Get all the handlers using the event type as the key
        foreach (var handler in scope.ServiceProvider.GetKeyedServices<IIntegrationEventHandler<IIntegrationEvent>>(eventType))
        {
            if (integrationEvent is not null)
                await handler.Handle(integrationEvent, default);
        }
        
        var rabbitRepository = scope.ServiceProvider.GetRequiredService<IMongoRepository<RabbitMessage>>();
        await rabbitRepository.InsertAsync(message);
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification = "The 'JsonSerializer.IsReflectionEnabledByDefault' feature switch, which is set to false by default for trimmed .NET apps, ensures the JsonSerializer doesn't use Reflection.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode", Justification = "See above.")]
    private IIntegrationEvent? DeserializeMessage(string message, Type eventType)
    {
        return JsonSerializer.Deserialize(
            message,
            eventType,
            _subscriptionInfo.JsonSerializerOptions) as IIntegrationEvent;
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        channel.Close();
        connection.Close();
    }
}