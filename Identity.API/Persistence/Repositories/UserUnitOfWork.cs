using System.Data;
using Application.Core.Abstractions;
using Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.API.Persistence.Repositories;

/// <summary>
/// Represents the user Unit of work class.
/// </summary>
internal sealed class UserUnitOfWork
    : IUserUnitOfWork
{
    private readonly UserDbContext _userDbContext;
    private bool _disposed;

    /// <summary>
    /// Initialize new instance of the <see cref="UserUnitOfWork"/>.
    /// </summary>
    /// <param name="userDbContext">The base generic db context.</param>
    public UserUnitOfWork(
        UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
        LastSaveChangesResult = new SaveChangesResult();
    }

     /// <inheritdoc />
    public async Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default,
        bool useIfExists = false)
    {
        IDbContextTransaction? transaction = _userDbContext.Database.CurrentTransaction;
            
        if (transaction == null)
        {
            return await _userDbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        return await (useIfExists ? Task.FromResult(transaction) : _userDbContext.Database.BeginTransactionAsync(cancellationToken));
    }

    /// <inheritdoc />
    public async Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default,
        bool useIfExists = false)
    {
        IDbContextTransaction? transaction = _userDbContext.Database.CurrentTransaction;
            
        if (transaction == null)
        {
            return await _userDbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        }

        return await (useIfExists ? Task.FromResult(transaction) : _userDbContext.Database.BeginTransactionAsync(cancellationToken));
    }

    /// <summary>
    /// DbContext disable/enable auto detect changes.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    public void SetAutoDetectChanges(bool value) =>
        _userDbContext.ChangeTracker.AutoDetectChangesEnabled = value;

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
            IExecutionStrategy strategy = _userDbContext.Database.CreateExecutionStrategy();
            //await strategy.ExecuteAsync(async () =>
            //{
                result = await _userDbContext.SaveChangesAsync(cancellationToken);
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
                _userDbContext.Dispose();
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
        _userDbContext.ChangeTracker.TrackGraph(rootEntity, callback);
}