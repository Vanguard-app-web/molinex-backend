namespace Molinex.Platform.Shared.Domain.Model.Events;

/// <summary>Event meant for other systems; stored in the outbox with the aggregate that produced it.</summary>
public interface IIntegrationEvent;
