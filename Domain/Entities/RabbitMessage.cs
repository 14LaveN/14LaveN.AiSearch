using Domain.Core.Primitives;

namespace Domain.Entities;

/// <summary>
/// Represents the RabbitMQ message class.
/// </summary>
public sealed class RabbitMessage 
    : BaseMongoEntity
{
    /// <summary>
    /// Gets or sets description.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Login the <see cref="RabbitMessage"/> class from <see cref="Description"/>.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns>New instance of the <see cref="RabbitMessage"/>.</returns>
    public static implicit operator RabbitMessage(string description)
    {
        return new RabbitMessage
        {
            Description = description
        };
    }
}