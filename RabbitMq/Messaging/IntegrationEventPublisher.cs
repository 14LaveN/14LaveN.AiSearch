using System.Text;
using System.Text.Json;
using TeamTasks.Application.Core.Abstractions.Messaging;
using TeamTasks.RabbitMq.Messaging.Settings;
using Microsoft.Extensions.Options;
using TeamTasks.Identity.IntegrationEvents.User.Events.PasswordChanged;
using TeamTasks.Identity.IntegrationEvents.User.Events.UserCreated;
using RabbitMQ.Client;
using RabbitMq.Converters;

namespace TeamTasks.RabbitMq.Messaging;

/// <summary>
/// Represents the integration event publisher.
/// </summary>
public sealed class IntegrationEventPublisher(
    IOptions<MessageBrokerSettings> messageBrokerSettingsOptions)
    : IIntegrationEventPublisher
{
    private readonly MessageBrokerSettings _messageBrokerSettings = messageBrokerSettingsOptions.Value;

    /// <summary>
    /// Initialize connection.
    /// </summary>
    /// <returns>Returns connection to <see cref="RabbitMQ"/>.</returns> 
    private static async Task<IConnection> CreateConnection()
    {
        ConnectionFactory connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(MessageBrokerSettings.AmqpLink)
        };

        var connection = await connectionFactory.CreateConnectionAsync();

        return connection;
    }

    /// <inheritdoc />
    public async Task Publish(IIntegrationEvent integrationEvent)
    {
        using var connection = await CreateConnection();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(_messageBrokerSettings.QueueName, false, false, false);

        await channel.ExchangeDeclareAsync(_messageBrokerSettings.QueueName + "Exchange", ExchangeType.Direct, durable: false);
        
        await channel.QueueBindAsync(_messageBrokerSettings.QueueName,
            exchange: _messageBrokerSettings.QueueName + "Exchange",
            routingKey: _messageBrokerSettings.QueueName);

        //TODO IIntegrationEvent concreteIntegrationEvent = integrationEvent.GetType().Name switch
        //TODO {
        //TODO     nameof(UserCreatedIntegrationEvent) =>
        //TODO         (UserCreatedIntegrationEvent)integrationEvent,
        //TODO     nameof(UserPasswordChangedIntegrationEvent) =>
        //TODO         (UserPasswordChangedIntegrationEvent)integrationEvent,
        //TODO     _ => integrationEvent
        //TODO };
        
        string payload = JsonSerializer.Serialize(concreteIntegrationEvent, new JsonSerializerOptions
        {
            Converters = { new IntegrationEventJsonConverter() }
        });

        byte[] body = Encoding.UTF8.GetBytes(payload);

        if (_messageBrokerSettings.QueueName is not null)
            await channel.BasicPublishAsync(_messageBrokerSettings.QueueName + "Exchange",
                _messageBrokerSettings.QueueName, body: body);
    }
}