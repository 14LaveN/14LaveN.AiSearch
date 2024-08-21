using MediatR;
using Application.ApiHelpers.Responses;
using Application.Core.Abstractions.Messaging;
using Domain.Common.Core.Primitives.Result;
using Domain.Core.Primitives.Result;

namespace Application.Core.Abstractions.Idempotency;

/// <summary>
/// Represents the idempotent command record.
/// </summary>
/// <param name="RequestId">The request identifier.</param>
public abstract record IdempotentCommand(Ulid RequestId)
    : ICommand<IBaseResponse<Result>>;

/// <summary>
/// Represents the idempotent command record.
/// </summary>
/// <param name="RequestId">The request identifier.</param>
/// <typeparam name="TValue">The generic type.</typeparam>
public abstract record IdempotentCommand<TValue>(Ulid RequestId)
    : ICommand<IBaseResponse<Result<TValue>>>;

