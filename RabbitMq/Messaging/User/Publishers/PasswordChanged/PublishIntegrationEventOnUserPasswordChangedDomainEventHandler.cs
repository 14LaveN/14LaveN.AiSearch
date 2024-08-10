using TeamTasks.Domain.Common.Core.Events;
using TeamTasks.Identity.Domain.Events.User;
using TeamTasks.Identity.IntegrationEvents.User.Events.PasswordChanged;

namespace TeamTasks.RabbitMq.Messaging.User.Publishers.PasswordChanged;

/// <summary>
/// Represents the <see cref="UserPasswordChangedDomainEvent"/> handler.
/// </summary>
internal sealed class PublishIntegrationEventOnUserPasswordChangedDomainEventHandler
    : IDomainEventHandler<UserPasswordChangedDomainEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishIntegrationEventOnUserPasswordChangedDomainEventHandler"/> class.
    /// </summary>
    /// <param name="integrationEventPublisher">The integration event publisher.</param>
    public PublishIntegrationEventOnUserPasswordChangedDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher) =>
        _integrationEventPublisher = integrationEventPublisher;

    /// <inheritdoc />
    public async Task Handle(UserPasswordChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _integrationEventPublisher.Publish(new UserPasswordChangedIntegrationEvent(notification));

        await Task.CompletedTask;
    }
}