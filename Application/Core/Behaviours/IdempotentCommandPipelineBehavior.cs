using Application.Core.Abstractions.Idempotency;
using MediatR;
using Application.Core.Abstractions.Idempotency;
using Application.Core.Abstractions.Messaging;

namespace Application.Core.Behaviours;

/// <summary>
/// Represents the identity idempotent command pipeline behavior class.
/// </summary>
/// <param name="idempotencyService">The idempotency service.</param>
/// <typeparam name="TRequest">The generic request type.</typeparam>
/// <typeparam name="TResponse">The generic response type.</typeparam>
public sealed class IdempotentCommandPipelineBehavior<TRequest, TResponse>(
    IIdempotencyService idempotencyService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IdempotentCommand
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