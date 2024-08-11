using Domain.Common.Core.Primitives.Maybe;
using Domain.Common.ValueObjects;
using Domain.ValueObjects;
using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

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
    Task<Maybe<User>> GetByIdAsync(Guid userId);
    
    /// <summary>
    /// Gets the user with the user name.
    /// </summary>
    /// <param name="name">The user name.</param>
    /// <returns>The maybe instance that may contain the user with the specified identifier.</returns>
    Task<Maybe<User>> GetByNameAsync(string name);

    /// <summary>
    /// Gets the user with the specified emailAddress.
    /// </summary>
    /// <param name="emailAddress">The user emailAddress.</param>
    /// <returns>The maybe instance that may contain the user with the specified emailAddress.</returns>
    Task<Maybe<User>> GetByEmailAsync(EmailAddress emailAddress);
}