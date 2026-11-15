using Molinex.Platform.ProductionManagement.Domain.Model.Events;
using Molinex.Platform.Shared.Application.Internal.EventHandlers;

namespace Molinex.Platform.ProductionManagement.Application.Internal.EventHandlers;

/// <summary>Reacts to Production Management domain events once their changes are saved.</summary>
public class ProductionEventHandlers(ILogger<ProductionEventHandlers> logger)
    : IEventHandler<RawMaterialReceptionRecordedEvent>,
        IEventHandler<ProductionBatchRegisteredEvent>,
        IEventHandler<ProductionInformationRecordedEvent>,
        IEventHandler<ProductionInformationUpdatedEvent>
{
    public Task Handle(RawMaterialReceptionRecordedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Raw material reception {ReceptionId} recorded", domainEvent.ReceptionId.Value);
        return Task.CompletedTask;
    }

    public Task Handle(ProductionBatchRegisteredEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Production batch {BatchId} ({BatchCode}) registered for reception {ReceptionId}",
            domainEvent.BatchId.Value, domainEvent.BatchCode, domainEvent.ReceptionId.Value);
        return Task.CompletedTask;
    }

    public Task Handle(ProductionInformationRecordedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Production record {RecordId} recorded for batch {BatchId}",
            domainEvent.RecordId.Value, domainEvent.BatchId.Value);
        return Task.CompletedTask;
    }

    public Task Handle(ProductionInformationUpdatedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Production record {RecordId} updated to status {Status}",
            domainEvent.RecordId.Value, domainEvent.Status);
        return Task.CompletedTask;
    }
}
