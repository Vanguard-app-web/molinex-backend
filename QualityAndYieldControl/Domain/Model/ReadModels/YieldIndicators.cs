using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.ReadModels;

/// <summary>Yield and waste of one production record. A missing yield is reported as unavailable, never as zero.</summary>
public sealed record YieldIndicator(
    ProductionRecordId ProductionRecordId,
    decimal? YieldPercentage,
    IReadOnlyList<decimal?> WastePercentages)
{
    public bool Available => YieldPercentage.HasValue;
}

public sealed record YieldIndicatorsReport(DateTime? From, DateTime? To, IReadOnlyList<YieldIndicator> Indicators);
