using System.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Core.Abstractions;
using Application.Core.Abstractions.Messaging;
using Domain.Common.Core.Errors;
using Identity.API.Domain.Repositories;
using Identity.API.Persistence;

namespace Identity.API.Mediatr.Behaviour;

/// <summary>
/// Represents the <see cref="DomainErrors.User"/> transaction behaviour class.
/// </summary>
internal sealed class UserTransactionBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    where TResponse : class
{
    private readonly IUserUnitOfWork _unitOfWork;
    private readonly UserDbContext _userDbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserTransactionBehavior{TRequest,TResponse}"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="userDbContext">The base database context.</param>
    public UserTransactionBehavior(
        IUserUnitOfWork unitOfWork,
        UserDbContext userDbContext)
    {
        _unitOfWork = unitOfWork;
        _userDbContext = userDbContext;
    }

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IQuery<TResponse>)
        {
            return await next();
        }
        TResponse response = await next();
        
        var strategy = _userDbContext.EfDatabase.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        });
        return response;
    }
}