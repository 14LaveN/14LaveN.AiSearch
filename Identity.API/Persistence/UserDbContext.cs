using System.Reflection;
using Application.Core.Abstractions;
using Domain.Common.Core.Abstractions;
using Domain.Common.Core.Primitives;
using Domain.Common.Core.Primitives.Maybe;
using Domain.Core.Extensions;
using Identity.Domain.Entities;
using Identity.Domain.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;

namespace Identity.API.Persistence;

/// <summary>
/// Represents the <see cref="User"/> database context base class.
/// </summary>
public sealed class UserDbContext
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public UserDbContext(
        DbContextOptions<UserDbContext> options)
        : base(options) { }

    /// <inheritdoc />
    public UserDbContext() { }
    
    /// <inheritdoc />
    public DatabaseFacade EfDatabase => Database;

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.ForeignKeyPropertiesMappedToUnrelatedTables));
        optionsBuilder
            .UseNpgsql();
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.Entity<IdentityUserLogin<Guid>>()
           .HasKey(l => new { l.LoginProvider, l.ProviderKey });

        modelBuilder.Entity<IdentityUserRole<Guid>>()
             .HasKey(l => new { l.UserId, l.RoleId });

        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .HasKey(l => new { l.UserId, l.LoginProvider, l.Name });

        modelBuilder.Entity<Category>()
            .HasNoKey();
    }

    /// <inheritdoc />
    public new DbSet<TEntity> Set<TEntity>()
        where TEntity : class
        => base.Set<TEntity>();

    /// <exception cref="ArgumentNullException"></exception>
    /// <inheritdoc />
    public async Task<Maybe<TEntity>> GetByIdAsync<TEntity>(Guid id)
        where TEntity : Entity
        => id == Guid.Empty ?
            Maybe<TEntity>.None :
            Maybe<TEntity>.From(await Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id) 
            ?? throw new ArgumentNullException());

    /// <inheritdoc />
    public async System.Threading.Tasks.Task Insert<TEntity>(TEntity entity)
        where TEntity : Entity
        => await Set<TEntity>().AddAsync(entity);

    /// <inheritdoc />
    public async System.Threading.Tasks.Task InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
        where TEntity : Entity
        => await Set<TEntity>().AddRangeAsync(entities);

    /// <inheritdoc />
    public new async Task Remove<TEntity>(TEntity? entity)
        where TEntity : Entity
        => await Set<TEntity>()
            .WhereIf(
                entity is not null, 
                e => e.Id == entity!.Id)
            .ExecuteDeleteAsync();
    
    /// <inheritdoc />
    public Task<int> ExecuteSqlAsync(string sql, IEnumerable<NpgsqlParameter> parameters, CancellationToken cancellationToken = default)
        => Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
    
   /// <summary>
   /// Saves all of the pending changes in the unit of work.
   /// </summary>
   /// <param name="cancellationToken">The cancellation token.</param>
   /// <returns>The number of entities that have been saved.</returns>
   public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
   {
       DateTime utcNow = DateTime.UtcNow;

       UpdateAuditableEntities(utcNow);
       UpdateSoftDeletableEntities(utcNow);

       return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates the entities implementing <see cref="IAuditableEntity"/> interface.
    /// </summary>
    /// <param name="utcNow">The current date and time in UTC format.</param>
    private void UpdateAuditableEntities(DateTime utcNow)
    {
        foreach (EntityEntry<IAuditableEntity> entityEntry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(nameof(IAuditableEntity.CreatedOnUtc)).CurrentValue = utcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(nameof(IAuditableEntity.ModifiedOnUtc)).CurrentValue = utcNow;
            }
        }
    }

        /// <summary>
        /// Updates the entities implementing <see cref="ISoftDeletableEntity"/> interface.
        /// </summary>
        /// <param name="utcNow">The current date and time in UTC format.</param>
        private void UpdateSoftDeletableEntities(DateTime utcNow)
        {
            foreach (EntityEntry<ISoftDeletableEntity> entityEntry in ChangeTracker.Entries<ISoftDeletableEntity>())
            {
                if (entityEntry.State != EntityState.Deleted)
                {
                    continue;
                }

                entityEntry.Property(nameof(ISoftDeletableEntity.DeletedOnUtc)).CurrentValue = utcNow;

                entityEntry.Property(nameof(ISoftDeletableEntity.Deleted)).CurrentValue = true;

                entityEntry.State = EntityState.Modified;

                UpdateDeletedEntityEntryReferencesToUnchanged(entityEntry);
            }
        }

        /// <summary>
        /// Updates the specified entity entry's referenced entries in the deleted state to the modified state.
        /// This method is recursive.
        /// </summary>
        /// <param name="entityEntry">The entity entry.</param>
        private static void UpdateDeletedEntityEntryReferencesToUnchanged(EntityEntry entityEntry)
        {
            if (!entityEntry.References.Any())
            {
                return;
            }

            foreach (ReferenceEntry referenceEntry in entityEntry.References
                         .Where(r => r.TargetEntry!.State == EntityState.Deleted))
            {
                if (referenceEntry.TargetEntry != null)
                {
                    referenceEntry.TargetEntry.State = EntityState.Unchanged;

                    UpdateDeletedEntityEntryReferencesToUnchanged(referenceEntry.TargetEntry);
                }
            }
        }

        /// <inheritdoc cref="FormattableString" />
        public async Task<int> ExecuteSqlAsync(FormattableString sql) =>
            await Database.ExecuteSqlAsync(sql);
}