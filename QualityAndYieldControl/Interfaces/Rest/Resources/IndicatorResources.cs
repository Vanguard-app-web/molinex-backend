namespace Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Resources;

public record YieldIndicatorResource(
    string ProductionRecordId,
    decimal? YieldPercentage,
    bool Available,
    IEnumerable<decimal?> WastePercentages);

public record YieldIndicatorsResource(
    DateTime? From,
    DateTime? To,
    string Unit,
    IEnumerable<YieldIndicatorResource> Results);

public record RiceCompositionItemResource(
    string ProductionRecordId,
    decimal WholeGrainPercentage,
    decimal BrokenGrainPercentage,
    DateTime AssessedAt);

public record RiceCompositionResource(string BatchId, bool Available, IEnumerable<RiceCompositionItemResource> Results);
