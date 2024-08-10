using System.Text;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeamTasks.BackgroundTasks.Services;
using TeamTasks.RabbitMq.Messaging.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TeamTasks.Application.Core.Abstractions.Messaging;
using TeamTasks.Database.MetricsAndRabbitMessages.Data.Interfaces;
using TeamTasks.Domain.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMq.Converters;
using ConnectionFactory = RabbitMQ.Client.ConnectionFactory;
using IChannel = RabbitMQ.Client.IChannel;
using IConnection = RabbitMQ.Client.IConnection;

namespace TeamTasks.BackgroundTasks.Tasks;

/// <summary>
/// Represents the integration event consumer background service class.
/// </summary>
internal  sealed class IntegrationEventConsumerBackgroundService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IChannel _channel;
    private readonly IConnection _connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationEventConsumerBackgroundService"/>
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="messageBrokerSettingsOptions">The message broker settings options.</param>
    public IntegrationEventConsumerBackgroundService(
        ILogger<IntegrationEventConsumerBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<MessageBrokerSettings> messageBrokerSettingsOptions)
    {
        _serviceProvider = serviceProvider;

        var messageBrokerSettings = messageBrokerSettingsOptions.Value;
        
        var factory = new ConnectionFactory
        {
            Uri = new Uri(MessageBrokerSettings.AmqpLink)
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(
            messageBrokerSettings.QueueName 
                                   ?? throw new InvalidOperationException(), 
            false, 
            false,
            false);

        try
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += OnIntegrationEventReceived!;

            _channel.BasicConsumeAsync(messageBrokerSettings.QueueName, false, consumer);
        }
        catch (Exception e)
        {
            logger.LogCritical($"ERROR: Failed to process the integration events: {e.Message}", e.Message);
        }
    }

    /// <inheritdoc />
    public System.Threading.Tasks.Task StartAsync(CancellationToken cancellationToken) => System.Threading.Tasks.Task.CompletedTask;

    /// <inheritdoc />
    public System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();

        return System.Threading.Tasks.Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _channel.CloseAsync();

        _connection.CloseAsync();
    }

    /// <summary>
    /// Processes the integration event received from the message queue.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="eventArgs">The event arguments.</param>
    /// <returns>The completed taskEntity.</returns>
    private void OnIntegrationEventReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        string body = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
        
        var integrationEvent = JsonConvert.DeserializeObject<IIntegrationEvent>(body, new JsonSerializerSettings
        {
            Converters = { new IntegrationEventConverter() },
            TypeNameHandling = TypeNameHandling.Auto
        });

        using IServiceScope scope = _serviceProvider.CreateScope();

        var integrationEventConsumer = scope.ServiceProvider.GetRequiredService<IIntegrationEventConsumer>();
        var rabbitRepository = scope.ServiceProvider.GetRequiredService<IMongoRepository<RabbitMessage>>();

        Task.FromResult(integrationEventConsumer.Consume(integrationEvent));
        Task.FromResult(rabbitRepository.InsertAsync(body));

        _channel.BasicAckAsync(eventArgs.DeliveryTag, false).GetAwaiter().GetResult();
    }
}