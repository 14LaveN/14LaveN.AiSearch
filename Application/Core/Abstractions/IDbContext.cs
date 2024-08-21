using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;
using Domain.Common.Core.Primitives;
using Domain.Common.Core.Primitives.Maybe;

namespace Application.Core.Abstractions;

/// <summary>
/// Represents the application database context interface.
/// </summary>
public interface IDbContext
{
    /// <summary>
    /// Gets database.
    /// </summary>
    DatabaseFacade EfDatabase { get; }
    
    /// <summary>
    /// Gets the database set for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The database set for the specified entity type.</returns>
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    /// <summary>
    /// Gets the entity with the specified identifier.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The entity identifier.</param>
    /// <returns>The <typeparamref name="TEntity"/> with the specified identifier if it exists, otherwise null.</returns>
    Task<Maybe<TEntity>> GetByIdAsync<TEntity>(Ulid id)
        where TEntity : Entity;
    
    /// <summary>
    /// Executes the specified SQL command asynchronously and returns the number of affected rows.
    /// </summary>
    /// <param name="sql">The SQL command.</param>
    /// <param name="parameters">The parameters collection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> ExecuteSqlAsync(
        string sql,
        IEnumerable<NpgsqlParameter> parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts the specified entity into the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entity">The entity to be inserted into the database.</param>
    System.Threading.Tasks.Task Insert<TEntity>(TEntity entity)
        where TEntity : Entity;

    /// <summary>
    /// Inserts the specified entities into the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entities">The entities to be inserted into the database.</param>
    System.Threading.Tasks.Task InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
        where TEntity : Entity;

    /// <summary>
    /// Removes the specified entity from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entity">The entity to be removed from the database.</param>
    Task Remove<TEntity>(TEntity? entity)
        where TEntity : Entity;
}
