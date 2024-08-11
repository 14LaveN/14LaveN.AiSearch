using System.Threading.Channels;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Events;

namespace Infrastructure.Events;

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