using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.ReadModels;

public sealed record RiceComposition(
    ProductionRecordId ProductionRecordId,
    decimal WholeGrainPercentage,
    decimal BrokenGrainPercentage,
    DateTime AssessedAt);

public sealed record RiceCompositionReport(long BatchId, IReadOnlyList<RiceComposition> Compositions)
{
    public bool Available => Compositions.Count > 0;
}
