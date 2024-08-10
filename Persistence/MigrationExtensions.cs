using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamTasks.Persistence;

namespace TeamTasks.Persistence;

/// <summary>
/// Represents the extensions for migration.
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Apply migrations.
    /// </summary>
    /// <param name="app">The web application.</param>
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<BaseDbContext>();

        dbContext.Database.MigrateAsync();
    }
}