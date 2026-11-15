using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.Events;

public record QualityAssessmentRecordedEvent(
    QualityAssessmentId AssessmentId,
    ProductionRecordId ProductionRecordId) : IEvent;

public record WasteRecordedEvent(WasteRecordId WasteRecordId, ProductionRecordId ProductionRecordId) : IEvent;
