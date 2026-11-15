namespace Molinex.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>Positive weight expressed in kilograms or metric tons. Comparable across units.</summary>
public sealed record Weight
{
    private const decimal KilogramsPerMetricTon = 1000m;

    public Weight(decimal value, MeasurementUnit unit)
    {
        Guard.Against(value <= 0, SharedErrors.InvalidWeight);
        Guard.Against(!Enum.IsDefined(unit), SharedErrors.InvalidMeasurementUnit);
        Value = value;
        Unit = unit;
    }

    public decimal Value { get; }
    public MeasurementUnit Unit { get; }

    public static Weight From(decimal value, string? unitCode) =>
        new(value, MeasurementUnitExtensions.FromCode(unitCode));

    public decimal ToKilograms() =>
        Unit == MeasurementUnit.MetricTon ? Value * KilogramsPerMetricTon : Value;

    public bool Exceeds(Weight other) => ToKilograms() > other.ToKilograms();
}
