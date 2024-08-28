using Domain.Common.Core.Abstractions;
using Domain.Core.Primitives;

namespace AiChat.API.Domain.Entities;

public sealed class Chat
    : AggregateRoot, IAuditableEntity, ISoftDeletableEntity
{
    /// <inheritdoc />
    public DateTime CreatedOnUtc { get; private set; }

    /// <inheritdoc />
    public DateTime? ModifiedOnUtc { get; private set; }

    /// <inheritdoc />
    public DateTime? DeletedOnUtc { get; private set; }

    /// <inheritdoc />
    public bool Deleted { get; private set; }
}