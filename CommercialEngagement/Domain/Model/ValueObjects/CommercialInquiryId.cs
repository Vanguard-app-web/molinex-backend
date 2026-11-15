using System.Globalization;

namespace Molinex.Platform.CommercialEngagement.Domain.Model.ValueObjects;

public readonly record struct CommercialInquiryId(long Value) : IEntityId<long>
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
