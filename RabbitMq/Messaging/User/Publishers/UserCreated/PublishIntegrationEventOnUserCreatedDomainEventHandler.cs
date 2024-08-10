using TeamTasks.Domain.Common.Core.Events;
using TeamTasks.Identity.Domain.Events.User;
using TeamTasks.Identity.IntegrationEvents.User.Events.UserCreated;

namespace TeamTasks.RabbitMq.Messaging.User.Publishers.UserCreated;

/// <summary>
/// Represents the <see cref="UserCreatedDomainEvent"/> handler.
/// </summary>
internal sealed class PublishIntegrationEventOnUserCreatedDomainEventHandler : IDomainEventHandler<UserCreatedDomainEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishIntegrationEventOnUserCreatedDomainEventHandler"/> class.
    /// </summary>
    /// <param name="integrationEventPublisher">The integration event publisher.</param>
    public PublishIntegrationEventOnUserCreatedDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher) =>
        _integrationEventPublisher = integrationEventPublisher;

    /// <inheritdoc />
    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        await _integrationEventPublisher.Publish(new UserCreatedIntegrationEvent(notification));
}