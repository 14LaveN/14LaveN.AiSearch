using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Identity.API.Persistence.Extensions;

/// <summary>
/// Represents the extensions for migration.
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Apply migrations.
    /// </summary>
    /// <param name="app">The web application.</param>
    public static void ApplyUserDbMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<UserDbContext>();

        dbContext.Database.MigrateAsync();
    }
}