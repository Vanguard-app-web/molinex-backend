using System.Globalization;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

/// <summary>
///     Identity of a production record as known by Quality and Yield Control.
///     It is this context's own type; the Production Management model is never referenced.
/// </summary>
public readonly record struct ProductionRecordId(long Value) : IEntityId<long>
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
