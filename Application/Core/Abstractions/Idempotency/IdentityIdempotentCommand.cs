using Application.ApiHelpers.Responses;
using TeamTasks.Application.ApiHelpers.Responses;
using TeamTasks.Application.Core.Abstractions.Messaging;
using TeamTasks.Domain.Core.Primitives.Result;
using TeamTasks.Identity.Domain.Entities;

namespace TeamTasks.Application.Core.Abstractions.Idempotency;

/// <summary>
/// Represents the identity idempotent command record.
/// </summary>
/// <param name="RequestId">The request identifier.</param>
public abstract record IdentityIdempotentCommand(Guid RequestId)
    : ICommand<LoginResponse<Result<User>>>;