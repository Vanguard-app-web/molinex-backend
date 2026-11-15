namespace Molinex.Platform.ProductionManagement.Interfaces.Acl;

/// <summary>Read contract of a production record. Carries primitive values only.</summary>
public sealed record ProductionRecordSummary(
    long ProductionRecordId,
    long ProductionBatchId,
    decimal ProcessedWeightValue,
    string ProcessedWeightUnit,
    DateTime StartedAt);
