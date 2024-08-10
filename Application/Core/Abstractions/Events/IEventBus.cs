using TeamTasks.Application.Core.Abstractions.Messaging;
using TeamTasks.Domain.Core.Events;

namespace TeamTasks.Application.Core.Abstractions.Events;

/// <summary>
/// Represents the event bus interface.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publish event with specified integration event.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="T">The generic integration event.</typeparam>
    /// <returns>Returns <see cref="Task"/>.</returns>
    Task PublishAsync<T>(
        T domainEvent,
        CancellationToken cancellationToken = default)
        where T : class, IDomainEvent;
}