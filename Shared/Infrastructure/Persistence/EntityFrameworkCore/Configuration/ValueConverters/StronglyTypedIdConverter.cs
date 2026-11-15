using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;

/// <summary>Maps a strongly typed identifier to its primitive database value.</summary>
public class StronglyTypedIdConverter<TId, TValue>(
    Func<TId, TValue> convertToProvider,
    Func<TValue, TId> convertFromProvider)
    : ValueConverter<TId, TValue>(
        id => convertToProvider(id),
        value => convertFromProvider(value))
    where TValue : struct
{
    public StronglyTypedIdConverter(Func<TValue, TId> factory)
        : this(id => ((IEntityId<TValue>)id!).Value, factory)
    {
    }
}
