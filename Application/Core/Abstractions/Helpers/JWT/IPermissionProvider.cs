using TeamTasks.Domain.Entities;

namespace TeamTasks.Application.Core.Abstractions.Helpers.JWT;

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