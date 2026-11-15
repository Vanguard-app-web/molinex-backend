using System.Globalization;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

public readonly record struct MaintenanceRecordId(long Value) : IEntityId<long>
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
