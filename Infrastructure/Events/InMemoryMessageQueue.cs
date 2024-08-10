using System.Threading.Channels;
using TeamTasks.Application.Core.Abstractions.Messaging;
using TeamTasks.Domain.Core.Events;

namespace TeamTasks.Infrastructure.Events;

/// <summary>
/// Represents the in memory message queue class.
/// </summary>
internal sealed class InMemoryMessageQueue
{
    private readonly Channel<IDomainEvent> _channel = Channel.CreateUnbounded<IDomainEvent>();

    /// <summary>
    /// Gets the channel writer.
    /// </summary>
    public ChannelWriter<IDomainEvent> Writer =>
        _channel.Writer;

    /// <summary>
    /// Gets the channel reader.
    /// </summary>
    public ChannelReader<IDomainEvent> Reader =>
        _channel.Reader;
}