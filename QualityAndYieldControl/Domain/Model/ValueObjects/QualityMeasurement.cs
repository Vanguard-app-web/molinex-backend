namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

/// <summary>Measured value of one quality indicator together with its expected range.</summary>
public sealed record QualityMeasurement
{
    private QualityMeasurement()
    {
        Value = null!;
        ExpectedRange = null!;
    }

    public QualityMeasurement(QualityIndicator indicator, Percentage value, QualityRange expectedRange)
    {
        Indicator = indicator;
        Value = value;
        ExpectedRange = expectedRange;
    }

    public QualityIndicator Indicator { get; private set; }
    public Percentage Value { get; private set; }
    public QualityRange ExpectedRange { get; private set; }

    public bool IsOutsideExpectedRange() => !ExpectedRange.Contains(Value);

    public static QualityMeasurement From(string? indicator, decimal value, decimal expectedMinimum,
        decimal expectedMaximum)
    {
        return new QualityMeasurement(
            QualityIndicatorExtensions.FromCode(indicator),
            new Percentage(value),
            new QualityRange(new Percentage(expectedMinimum), new Percentage(expectedMaximum)));
    }
}
