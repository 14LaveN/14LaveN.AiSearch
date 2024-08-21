namespace Application.Core.Abstractions.Idempotency;

/// <summary>
/// Represents the idempotency service interface. 
/// </summary>
public interface IIdempotencyService
{
    /// <summary>
    /// Request exists method with specified request identifier.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <returns>Returns boolean value if request exists.</returns>
    Task<bool> RequestExistsAsync(Ulid requestId);

    /// <summary>
    /// Create the request with specified request identifier and name.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="name">The name.</param>
    /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
    Task CreateRequestAsync(Ulid requestId, string name);
}