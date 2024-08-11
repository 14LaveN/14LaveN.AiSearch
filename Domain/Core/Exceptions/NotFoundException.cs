namespace Domain.Core.Exceptions;

/// <summary>
/// Represents the not found <see cref="Exception"/> class.
/// </summary>
public sealed class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="NotFoundException"/>.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="key">The key.</param>
    public NotFoundException(string name, object key)
        : base($"Entity {name} ({key}) not found.") { }
}