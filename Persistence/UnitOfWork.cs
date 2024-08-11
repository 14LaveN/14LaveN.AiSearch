using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence;
using Application.Core.Abstractions;
using Persistence;

namespace Persistence;

/// <summary>
/// Represents the generic unit of work.
/// </summary>
public sealed class UnitOfWork
    : IUnitOfWork
{
    private readonly BaseDbContext _baseDbContext;
    private bool _disposed;

    /// <summary>
    /// Initialize generic db context.
    /// </summary>
    /// <param name="baseDbContext">The base generic db context.</param>
    /// <param name="lastSaveChangesResult">The last save changes result.</param>
    public UnitOfWork(
        BaseDbContext baseDbContext,
        SaveChangesResult lastSaveChangesResult)
    {
        _baseDbContext = baseDbContext;
        LastSaveChangesResult = new SaveChangesResult();
    }

     /// <inheritdoc />
    public async Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default,
        bool useIfExists = false)
    {
        IDbContextTransaction? transaction = _baseDbContext.Database.CurrentTransaction;
            
        if (transaction == null)
        {
            return await _baseDbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        return await (useIfExists ? Task.FromResult(transaction) : _baseDbContext.Database.BeginTransactionAsync(cancellationToken));
    }

    /// <summary>
    /// DbContext disable/enable auto detect changes.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    public void SetAutoDetectChanges(bool value) =>
        _baseDbContext.ChangeTracker.AutoDetectChangesEnabled = value;

    public SaveChangesResult LastSaveChangesResult { get; }
    
    /// <summary>
    /// Asynchronously saves all changes made in this unit of work to the database.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            int result = 0;
            IExecutionStrategy strategy = _baseDbContext.Database.CreateExecutionStrategy();
            //await strategy.ExecuteAsync(async () =>
            //{
                result = await _baseDbContext.SaveChangesAsync(cancellationToken);
            //});

            return result;
        }
        catch (Exception exception)
        {
            LastSaveChangesResult.Exception = exception;
            return 0;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        //ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">The disposing.</param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _baseDbContext.Dispose();
            }
        }
        _disposed = true;
    }

    /// <summary>
    /// Uses Track Graph Api to attach disconnected entities
    /// </summary>
    /// <param name="rootEntity"> Root entity</param>
    /// <param name="callback">Delegate to convert Object's State properties to Entities entry state.</param>
    public void TrackGraph(
        object rootEntity,
        Action<EntityEntryGraphNode> callback) =>
        _baseDbContext.ChangeTracker.TrackGraph(rootEntity, callback);
}