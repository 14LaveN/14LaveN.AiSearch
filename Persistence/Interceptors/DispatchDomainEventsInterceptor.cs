using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TeamTasks.Domain.Core.Primitives;

namespace Persistence.Interceptors;

/// <summary>
/// Represents the dispatch domain events interceptor class.
/// </summary>
/// <param name="publisher">The publisher.</param>
internal sealed class DispatchDomainEventsInterceptor(IPublisher publisher) 
    : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();

        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async System.Threading.Tasks.Task DispatchDomainEvents(DbContext? context)
    {
        if (context is null) return;
        
        var entities = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count is not 0)
            .Select(e => e.Entity);

        var auditableBaseEntities = entities as AggregateRoot[] 
                                    ?? entities.ToArray();
        
        var domainEvents = auditableBaseEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        auditableBaseEntities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent);
    }
}