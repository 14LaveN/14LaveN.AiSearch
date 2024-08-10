using Application.Core.Abstractions.Idempotency;
using MediatR;
using TeamTasks.Application.Core.Abstractions.Idempotency;
using TeamTasks.Application.Core.Abstractions.Messaging;

namespace TeamTasks.Application.Core.Behaviours;

/// <summary>
/// Represents the identity idempotent command pipeline behavior class.
/// </summary>
/// <param name="idempotencyService">The idempotency service.</param>
/// <typeparam name="TRequest">The generic request type.</typeparam>
/// <typeparam name="TResponse">The generic response type.</typeparam>
public sealed class IdentityIdempotentCommandPipelineBehavior<TRequest, TResponse>(
    IIdempotencyService idempotencyService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IdentityIdempotentCommand
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (await idempotencyService.RequestExistsAsync(request.RequestId))
        {
            return default;
        }
        
        await idempotencyService.CreateRequestAsync(request.RequestId, typeof(TRequest).Name);
        
        TResponse response = await next();

        return response;
    }
}