using System.Text.Json;
using Molinex.Platform.Shared.Application.Internal.OutboundServices;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Outbox;

public sealed class IntegrationEventOutbox(AppDbContext context, TimeProvider timeProvider) : IIntegrationEventOutbox
{
    public async Task EnqueueAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        var message = new OutboxMessage(typeof(TEvent).Name, JsonSerializer.Serialize(integrationEvent),
            timeProvider.GetUtcNow().UtcDateTime);
        await context.Set<OutboxMessage>().AddAsync(message, cancellationToken);
    }
}
