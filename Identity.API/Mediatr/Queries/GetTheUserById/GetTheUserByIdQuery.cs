using Identity.API.Contracts.Get;
using Application.Core.Abstractions.Messaging;
using Domain.Common.Core.Primitives.Maybe;

namespace Identity.API.Mediatr.Queries.GetTheUserById;

/// <summary>
/// Represents the get user by id query record.
/// </summary>
/// <param name="UserId">The user identifier.</param>
public sealed record GetTheUserByIdQuery(Ulid UserId)
    : ICachedQuery<Maybe<GetUserResponse>>
{
    public string Key { get; } = $"get-user-by-{UserId}";
    
    public TimeSpan? Expiration { get; } = TimeSpan.FromMinutes(6);
}