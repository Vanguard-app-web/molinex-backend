using Cortex.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;

/// <summary>
///     After a successful save, collects the pending domain events of every tracked aggregate,
///     clears them and publishes them through the mediator.
/// </summary>
public sealed class DispatchDomainEventsInterceptor(
    IMediator mediator,
    ILogger<DispatchDomainEventsInterceptor> logger) : SaveChangesInterceptor
{
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context, cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEvents(DbContext? context, CancellationToken cancellationToken = default)
    {
        if (context is null) return;
        var aggregateRoots = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();
        if (aggregateRoots.Count == 0) return;

        var domainEvents = aggregateRoots.SelectMany(aggregate => aggregate.DomainEvents).ToList();
        foreach (var aggregateRoot in aggregateRoots) aggregateRoot.ClearDomainEvents();

        // The dynamic cast lets the mediator resolve handlers for the concrete event type.
        foreach (var domainEvent in domainEvents)
            await mediator.PublishAsync((dynamic)domainEvent, cancellationToken);

        logger.LogInformation(
            "Dispatched {EventCount} domain event(s) [{EventNames}]; pending after dispatch: {PendingCount}",
            domainEvents.Count,
            string.Join(", ", domainEvents.Select(domainEvent => domainEvent.GetType().Name)),
            aggregateRoots.Sum(aggregate => aggregate.DomainEvents.Count));
    }
}
