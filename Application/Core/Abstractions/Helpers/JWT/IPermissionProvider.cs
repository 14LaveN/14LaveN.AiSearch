using Domain.Entities;

namespace Application.Core.Abstractions.Helpers.JWT;

/// <summary>
/// Represents the permission provider interface.
/// </summary>
public interface IPermissionProvider
{
    /// <summary>
    /// Gets the permissions.
    /// </summary>
    HashSet<string> Permissions { get; }
}