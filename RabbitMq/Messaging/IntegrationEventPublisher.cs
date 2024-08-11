using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Application.Core.Abstractions.Messaging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMq.Abstractions;
using RabbitMq.Abstractions.Polly;
using RabbitMq.Abstractions.Settings;
using RabbitMq.Extensions;

namespace RabbitMq.Messaging;

/// <summary>
/// Represents the integration event publisher.
/// </summary>
public sealed class IntegrationEventPublisher(
    ILogger<IntegrationEventPublisher> logger,
    IOptions<MessageBrokerSettings> messageBrokerSettingsOptions,
    IOptions<EventBusSubscriptionInfo> subscriptionOptions,
    RabbitMqTelemetry rabbitMqTelemetry)
    : IIntegrationEventPublisher
{
    private readonly MessageBrokerSettings _messageBrokerSettings = messageBrokerSettingsOptions.Value;

    private readonly ResiliencePipeline _pipeline = 
        RabbitResiliencePipeline.CreateResiliencePipeline(10);
    
    private readonly TextMapPropagator _propagator = rabbitMqTelemetry.Propagator;
    private readonly ActivitySource _activitySource = rabbitMqTelemetry.ActivitySource;
    
    private readonly EventBusSubscriptionInfo _subscriptionInfo = subscriptionOptions.Value;
    
    /// <summary>
    /// Initialize connection.
    /// </summary>
    /// <returns>Returns connection to <see cref="RabbitMQ"/>.</returns> 
    private static IConnection CreateConnection()
    {
        ConnectionFactory connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(MessageBrokerSettings.AmqpLink)
        };

        var connection = connectionFactory.CreateConnection();

        return connection;
    }

    public Task Publish(IIntegrationEvent @event)
    {
        var routingKey = @event.GetType().Name;

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, routingKey);
        }

        using var channel = CreateConnection().CreateModel() 
                            ?? throw new InvalidOperationException("RabbitMQ connection is not open");

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
        }

        channel.ExchangeDeclare(
            exchange: _messageBrokerSettings + "Exchange", 
            type: ExchangeType.Direct);

        var body = SerializeMessage(@event);

        // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
        var activityName = $"{routingKey} publish";

        return _pipeline.Execute(() =>
        {
            using var activity = _activitySource.StartActivity(activityName, ActivityKind.Client);

            // Depending on Sampling (and whether a listener is registered or not), the activity above may not be created.
            // If it is created, then propagate its context. If it is not created, the propagate the Current context, if any.

            ActivityContext contextToInject = default;

            if (activity is not null)
                contextToInject = activity.Context;
            else if (Activity.Current is not null)
                contextToInject = Activity.Current.Context;
            

            var properties = channel.CreateBasicProperties();
            // persistent
            properties.DeliveryMode = 2;

            static void InjectTraceContextIntoBasicProperties(IBasicProperties props, string key, string value)
            {
                props.Headers ??= new Dictionary<string, object>()!;
                props.Headers[key] = value;
            }

            _propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), properties, InjectTraceContextIntoBasicProperties);

            ActivityExtensions.SetActivityContext(activity, routingKey, "publish");

            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);
            }

            try
            {
                channel.BasicPublish(
                    exchange: _messageBrokerSettings.QueueName + "Exchange",
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                activity.SetExceptionTags(ex);

                throw;
            }
        });
    }
    
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification = "The 'JsonSerializer.IsReflectionEnabledByDefault' feature switch, which is set to false by default for trimmed .NET apps, ensures the JsonSerializer doesn't use Reflection.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode", Justification = "See above.")]
    private byte[] SerializeMessage(IIntegrationEvent @event)
    {
        return JsonSerializer.SerializeToUtf8Bytes(
            @event, 
            @event.GetType(),
            _subscriptionInfo.JsonSerializerOptions);
    }
}