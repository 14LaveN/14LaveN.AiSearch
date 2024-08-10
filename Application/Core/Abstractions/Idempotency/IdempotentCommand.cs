using MediatR;
using TeamTasks.Application.ApiHelpers.Responses;
using TeamTasks.Application.Core.Abstractions.Messaging;
using TeamTasks.Domain.Common.Core.Primitives.Result;
using TeamTasks.Domain.Core.Primitives.Result;

namespace TeamTasks.Application.Core.Abstractions.Idempotency;

/// <summary>
/// Represents the idempotent command record.
/// </summary>
/// <param name="RequestId">The request identifier.</param>
public abstract record IdempotentCommand(Guid RequestId)
    : ICommand<IBaseResponse<Result>>;

/// <summary>
/// Represents the idempotent command record.
/// </summary>
/// <param name="RequestId">The request identifier.</param>
/// <typeparam name="TValue">The generic type.</typeparam>
public abstract record IdempotentCommand<TValue>(Guid RequestId)
    : ICommand<IBaseResponse<Result<TValue>>>;

