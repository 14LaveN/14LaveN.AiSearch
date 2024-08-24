using System.Data;
using Application.Core.Abstractions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.API.Domain.Repositories;

public interface IUserUnitOfWork
    : IDisposable
{
    /// <summary>
    /// Saves all of the pending changes in the unit of work.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>

    /// <returns>The number of entities that have been saved.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a transaction on the current unit of work.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="useIfExists">The use if exists.</param>
    /// <returns>The new database context transaction.</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default,
        bool useIfExists = false);

    /// <summary>
    /// Begins a transaction on the current unit of work.
    /// </summary>
    /// <param name="isolationLevel">The isolation level.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="useIfExists">The use if exists.</param>
    /// <returns>The new database context transaction.</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default,
        bool useIfExists = false);
    
    /// <summary>
    /// Uses Track Graph Api to attach disconnected entities
    /// </summary>
    /// <param name="rootEntity"> Root entity</param>
    /// <param name="callback">Delegate to convert Object's State properties to Entities entry state.</param>
    void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback);
    
    /// <summary>
    /// DbContext disable/enable auto detect changes
    /// </summary>
    /// <param name="value"></param>
    void SetAutoDetectChanges(bool value);

    /// <summary>
    /// Last error after SaveChanges operation executed
    /// </summary>
    SaveChangesResult LastSaveChangesResult { get; }
}