using Application.Core.Abstractions.Messaging;
using Domain.Core.Primitives.Result;
using Identity.API.Common.ApiHelpers.Responses;
using Identity.API.Domain.Entities;

namespace Identity.API.Common.Abstractions.Idempotency;

/// <summary>
/// Represents the identity idempotent command record.
/// </summary>
/// <param name="RequestId">The request identifier.</param>
public abstract record IdentityIdempotentCommand(Ulid RequestId)
    : ICommand<LoginResponse<Result<User>>>;