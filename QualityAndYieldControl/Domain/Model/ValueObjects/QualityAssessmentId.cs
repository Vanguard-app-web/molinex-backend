using System.Globalization;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

public readonly record struct QualityAssessmentId(long Value) : IEntityId<long>
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
