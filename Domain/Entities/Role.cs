using Domain.Common.Core.Primitives;
using Domain.Core.Primitives;

namespace Domain.Entities;

/// <summary>
/// Represents the role entity.
/// </summary>
/// <param name="id">The identifier.</param>
/// <param name="name">The name.</param>
public sealed class Role(
    int id,
    string name) : Enumeration<Role>(id, name)
{
    /// <summary>
    /// The ef core ctor.
    /// </summary>
    public Role() : this(0, String.Empty)
    {}
    
    /// <summary>
    /// The registered <see cref="Role"/>.
    /// </summary>
    public static readonly Role Registered = new Role(1,"Registered");

    /// <summary>
    /// Gets or sets permissions.
    /// </summary>
    public ICollection<Permission> Permissions { get; set; }
}