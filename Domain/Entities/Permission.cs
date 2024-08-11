using Domain.Core.Utility;

namespace Domain.Entities;

/// <summary>
/// Represents the permission entity.
/// </summary>
public sealed class Permission
{
    /// <summary>
    /// Initializes a new instance of <see cref="Permission"/>.
    /// </summary>
    /// <param name="name">The name.</param>
    public Permission(string name)
    {
        Ensure.NotEmpty(name, "Name is required.", nameof(name));
        
        Name = name;
    }
    
    /// <summary>
    /// Gets or inits identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets or inits name.
    /// </summary>
    public string Name { get; init; }
}