namespace Domain.Core.Exceptions;

/// <summary>
/// Represents the guid parse <see cref="Exception"/> class.
/// </summary>
public sealed class GuidParseException
    : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="GuidParseException"/>.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="key">The key.</param>
    public GuidParseException(string name, object key)
        : base($"Entity {name} ({key}) not parsed.") { }
}