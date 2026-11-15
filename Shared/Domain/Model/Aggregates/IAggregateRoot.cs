namespace Molinex.Platform.Shared.Domain.Model.Aggregates;

public interface IAggregateRoot
{
    IReadOnlyCollection<IEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
