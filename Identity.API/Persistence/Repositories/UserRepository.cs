using Microsoft.EntityFrameworkCore;
using Application.Core.Abstractions;
using Dapper;
using Domain.Common.Core.Primitives.Maybe;
using Domain.ValueObjects;
using Identity.API.Domain.Entities;
using Identity.API.Domain.Repositories;
using Persistence;

namespace Identity.API.Persistence.Repositories;

/// <summary>
/// Represents the user repository class.
/// </summary>
/// <param name="userDbContext">The user database context.</param>
internal class UserRepository(UserDbContext userDbContext)
    : IUserRepository
{
    /// <inheritdoc />
    public async Task<Maybe<User>> GetByIdAsync(Ulid userId) =>
            await userDbContext
                .Set<User>()
                .AsNoTracking()
                .AsSplitQuery()
                .SingleOrDefaultAsync(x=>x.Id == userId) 
            ?? throw new ArgumentNullException();

    /// <inheritdoc />
    public async Task<Maybe<IEnumerable<(string UserName, string RoleName)>>> GetUsersJoin()
    {
        await using var connection = DbConnection.CreateConnection();

        await connection.OpenAsync();
        
        var sql = $"""
                   SELECT u."UserName", r."Name"
                  FROM dbo."users" AS u
                  LEFT JOIN dbo."roles" AS r ON u."LastName" = 'dfdsfdsfsd'
                  GROUP BY u."UserName", r."Name"
                  ORDER BY u."UserName" ASC
                  LIMIT 3
                  """;

        var result = await connection.QueryAsync<(string, string)>(sql);
        
        return Maybe<IEnumerable<(string, string)>>.From(result);
    }
    
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