using Molinex.Platform.QualityAndYieldControl.Domain.Model.Errors;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

public enum QualityIndicator
{
    WholeGrainPercentage,
    BrokenGrainPercentage,
    YieldPercentage
}

public static class QualityIndicatorExtensions
{
    public static string ToCode(this QualityIndicator indicator)
    {
        return indicator switch
        {
            QualityIndicator.WholeGrainPercentage => "WHOLE_GRAIN_PERCENTAGE",
            QualityIndicator.BrokenGrainPercentage => "BROKEN_GRAIN_PERCENTAGE",
            _ => "YIELD_PERCENTAGE"
        };
    }

    public static QualityIndicator FromCode(string? code)
    {
        return code?.Trim() switch
        {
            "WHOLE_GRAIN_PERCENTAGE" => QualityIndicator.WholeGrainPercentage,
            "BROKEN_GRAIN_PERCENTAGE" => QualityIndicator.BrokenGrainPercentage,
            "YIELD_PERCENTAGE" => QualityIndicator.YieldPercentage,
            _ => throw new DomainRuleException(QualityErrors.InvalidIndicator)
        };
    }
}
