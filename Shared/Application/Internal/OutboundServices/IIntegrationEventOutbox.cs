namespace Molinex.Platform.Shared.Application.Internal.OutboundServices;

/// <summary>
///     Port used by application services to store an integration event in the same unit of work
///     as the aggregate that produced it.
/// </summary>
public interface IIntegrationEventOutbox
{
    Task EnqueueAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
