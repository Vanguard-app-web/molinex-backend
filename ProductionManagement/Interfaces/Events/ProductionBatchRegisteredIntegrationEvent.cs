namespace Molinex.Platform.ProductionManagement.Interfaces.Events;

/// <summary>Integration event stored in the outbox when a production batch is registered.</summary>
public sealed record ProductionBatchRegisteredIntegrationEvent(
    string BatchCode,
    long ReceptionId,
    DateTime RegisteredAt) : IIntegrationEvent;
