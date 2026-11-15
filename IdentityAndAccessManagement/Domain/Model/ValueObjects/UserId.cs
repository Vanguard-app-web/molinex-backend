using System.Globalization;

namespace Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;

public readonly record struct UserId(long Value) : IEntityId<long>
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
