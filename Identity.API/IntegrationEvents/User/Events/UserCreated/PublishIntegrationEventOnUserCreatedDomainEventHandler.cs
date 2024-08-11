using Domain.Common.Core.Events;
using Identity.Domain.Events.User;
using RabbitMq.Messaging;

namespace Identity.API.IntegrationEvents.User.Events.UserCreated;

/// <summary>
/// Represents the <see cref="UserCreatedDomainEvent"/> handler.
/// </summary>
internal sealed class PublishIntegrationEventOnUserCreatedDomainEventHandler 
    : IDomainEventHandler<UserCreatedDomainEvent>
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