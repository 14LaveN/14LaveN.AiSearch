namespace Persistence.Idempotency;

/// <summary>
/// Represents the idempotent request class.
/// </summary>
internal sealed class IdempotentRequest
{
    /// <summary>
    /// Gets or sets identifier.
    /// </summary>
    public Ulid Id { get; set; }

    /// <summary>
    /// Gets or sets name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets date/time creation.
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}