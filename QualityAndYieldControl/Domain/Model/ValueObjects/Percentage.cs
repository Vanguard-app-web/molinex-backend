using Molinex.Platform.QualityAndYieldControl.Domain.Model.Errors;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

public sealed record Percentage
{
    public const decimal Minimum = 0m;
    public const decimal Maximum = 100m;

    public Percentage(decimal value)
    {
        Guard.Against(value is < Minimum or > Maximum, QualityErrors.InvalidPercentage);
        Value = value;
    }

    public decimal Value { get; }
}
