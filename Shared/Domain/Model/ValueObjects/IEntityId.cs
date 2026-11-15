namespace Molinex.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>Contract for strongly typed aggregate identifiers.</summary>
public interface IEntityId<out T> where T : struct
{
    T Value { get; }
}
