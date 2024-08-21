namespace Domain.Core.Exceptions;

/// <summary>
/// Represents the guid parse <see cref="Exception"/> class.
/// </summary>
public sealed class UlidParseException
    : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="UlidParseException"/>.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="key">The key.</param>
    public UlidParseException(string name, object key)
        : base($"Entity {name} ({key}) not parsed.") { }
}