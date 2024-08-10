using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeamTasks.Application.Core.Abstractions.Messaging;
using TeamTasks.Domain.Core.Events;

namespace TeamTasks.Infrastructure.Events;

/// <summary>
/// Represents the integration event processor job class.
/// </summary>
/// <param name="queue">The in memory message queue.</param>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="logger">The logger.</param>
internal sealed class IntegrationEventProcessorJob(
    InMemoryMessageQueue queue,
    IServiceProvider serviceProvider,
    ILogger<IntegrationEventProcessorJob> logger) 
    : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IServiceScope scope = serviceProvider.CreateScope();
        IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
        
        await foreach (IDomainEvent integrationEvent in queue.Reader.ReadAllAsync(stoppingToken))
        {
            logger.LogInformation($"Publishing {integrationEvent}");

            await publisher.Publish(integrationEvent, stoppingToken);
            
            logger.LogInformation($"Processed {integrationEvent}");
        }
    }
}