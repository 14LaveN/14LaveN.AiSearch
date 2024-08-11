using Microsoft.EntityFrameworkCore;
using Application.Core.Abstractions;
using Domain.Common.Core.Primitives.Maybe;
using Domain.ValueObjects;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;

namespace Identity.API.Persistence.Repositories;

/// <summary>
/// Represents the user repository class.
/// </summary>
/// <param name="userDbContext">The user database context.</param>
public class UserRepository(IDbContext userDbContext)
    : IUserRepository
{
    /// <inheritdoc />
    public async Task<Maybe<User>> GetByIdAsync(Guid userId) =>
            await userDbContext
                .Set<User>()
                .AsNoTracking()
                .AsSplitQuery()
                .SingleOrDefaultAsync(x=>x.Id == userId) 
            ?? throw new ArgumentNullException();

    /// <inheritdoc />
    public async Task<Maybe<User>> GetByNameAsync(string name) =>
        await userDbContext
            .Set<User>()
            .AsNoTracking()
            .AsSplitQuery()
            .SingleOrDefaultAsync(x=>x.UserName == name) 
        ?? throw new ArgumentNullException();

    /// <inheritdoc />
    public async Task<Maybe<User>> GetByEmailAsync(EmailAddress emailAddress) =>
        await userDbContext
            .Set<User>()
            .SingleOrDefaultAsync(x=>x.EmailAddress == emailAddress) 
        ?? throw new ArgumentNullException();
}