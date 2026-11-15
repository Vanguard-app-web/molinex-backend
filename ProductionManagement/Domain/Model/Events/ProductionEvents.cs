using Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.ProductionManagement.Domain.Model.Events;

public record RawMaterialReceptionRecordedEvent(RawMaterialReceptionId ReceptionId, DateTime ReceivedAt) : IEvent;

public record ProductionBatchRegisteredEvent(
    ProductionBatchId BatchId,
    string BatchCode,
    RawMaterialReceptionId ReceptionId) : IEvent;

public record ProductionInformationRecordedEvent(ProductionRecordId RecordId, ProductionBatchId BatchId) : IEvent;

public record ProductionInformationUpdatedEvent(ProductionRecordId RecordId, string Status) : IEvent;
