namespace Molinex.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>Weight units shared by Production Management and Quality and Yield Control.</summary>
public enum MeasurementUnit
{
    Kilogram,
    MetricTon
}

public static class MeasurementUnitExtensions
{
    public const string KilogramCode = "KILOGRAM";
    public const string MetricTonCode = "METRIC_TON";

    public static string ToCode(this MeasurementUnit unit) =>
        unit == MeasurementUnit.MetricTon ? MetricTonCode : KilogramCode;

    public static MeasurementUnit FromCode(string? code)
    {
        return code?.Trim() switch
        {
            KilogramCode => MeasurementUnit.Kilogram,
            MetricTonCode => MeasurementUnit.MetricTon,
            _ => throw new DomainRuleException(SharedErrors.InvalidMeasurementUnit)
        };
    }
}
