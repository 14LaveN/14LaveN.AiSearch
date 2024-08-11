namespace Domain.Entities;

/// <summary>
/// Represents the role permission entity.
/// </summary>
public sealed class RolePermission
{
    /// <summary>
    /// Gets or inits role identifier.
    /// </summary>
    public int RoleId { get; init; }

    /// <summary>
    /// Gets or inits permission identifier.
    /// </summary>
    public int PermissionId { get; init; }
}