using Domain.Common.Core.Primitives.Maybe;
using Domain.ValueObjects;
using Identity.API.Domain.Entities;

namespace Identity.API.Domain.Repositories;

/// <summary>
/// Represents the user repository interface.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets the user with the specified identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The maybe instance that may contain the user with the specified identifier.</returns>
    Task<Maybe<User>> GetByIdAsync(Ulid userId);
    
    /// <summary>
    /// Gets the user with the user name.
    /// </summary>
    /// <param name="name">The user name.</param>
    /// <returns>The maybe instance that may contain the user with the specified identifier.</returns>
    Task<Maybe<User>> GetByNameAsync(string name);

    /// <summary>
    /// Gets users.
    /// </summary>
    /// <returns>The maybe queryable of users.</returns>
    Task<Maybe<IEnumerable<(string UserName, string RoleName)>>> GetUsersJoin();
    
    /// <summary>
    /// Gets the user with the specified emailAddress.
    /// </summary>
    /// <param name="emailAddress">The user emailAddress.</param>
    /// <returns>The maybe instance that may contain the user with the specified emailAddress.</returns>
    Task<Maybe<User>> GetByEmailAsync(EmailAddress emailAddress);
}