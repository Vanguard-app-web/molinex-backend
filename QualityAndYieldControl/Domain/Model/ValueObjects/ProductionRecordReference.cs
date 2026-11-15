namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

/// <summary>What Quality and Yield Control needs to know about a production record.</summary>
public sealed record ProductionRecordReference(
    ProductionRecordId Id,
    long ProductionBatchId,
    Weight ProcessedWeight,
    DateTime StartedAt);
