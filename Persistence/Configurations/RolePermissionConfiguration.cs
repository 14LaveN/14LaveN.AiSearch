using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTasks.Domain.Entities;
using Permission = TeamTasks.Domain.Enumerations.Permission;

namespace TeamTasks.Persistence.Configurations;

/// <summary>
/// Represents the <see cref="RolePermission"/> configuration.
/// </summary>
internal sealed class RolePermissionConfiguration
    : IEntityTypeConfiguration<RolePermission>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(rp => new {rp.RoleId, rp.PermissionId});

        builder.HasData(
            Create(Role.Registered, Permission.ReadMember),
            Create(Role.Registered, Permission.UpdateMember));
    }

    private static RolePermission Create(
        Role role,
        Permission permission)
    {
        return new RolePermission
        {
            RoleId = role.Value,
            PermissionId = (int)permission
        };
    }
}