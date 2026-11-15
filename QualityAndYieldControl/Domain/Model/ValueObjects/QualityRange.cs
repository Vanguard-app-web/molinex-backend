using Molinex.Platform.QualityAndYieldControl.Domain.Model.Errors;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

public sealed record QualityRange
{
    public QualityRange(Percentage minimum, Percentage maximum)
    {
        Guard.Against(minimum.Value > maximum.Value, QualityErrors.InvalidQualityRange);
        Minimum = minimum;
        Maximum = maximum;
    }

    public Percentage Minimum { get; }
    public Percentage Maximum { get; }

    public bool Contains(Percentage value) => value.Value >= Minimum.Value && value.Value <= Maximum.Value;
}
