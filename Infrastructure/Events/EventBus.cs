using Application.Core.Abstractions.Events;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Events;

namespace Infrastructure.Events;

/// <summary>
/// Represents the event bus class.
/// </summary>
/// <param name="queue">The in memory message queue.</param>>
internal sealed class EventBus(InMemoryMessageQueue queue)
    : IEventBus
{
    /// <inehritdoc />
    public async Task PublishAsync<T>(
        T domainEvent,
        CancellationToken cancellationToken = default)
        where T : class, IDomainEvent
    {
        await queue.Writer.WriteAsync(domainEvent, cancellationToken);
    }
}