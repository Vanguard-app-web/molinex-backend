using System.Globalization;

namespace Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

public readonly record struct ProductionBatchId(long Value) : IEntityId<long>
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
