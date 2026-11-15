using Molinex.Platform.Shared.Domain.Model.Entities;

namespace Molinex.Platform.Shared.Domain.Model.Aggregates;

/// <summary>Aggregate whose creation and update instants are stamped by the persistence layer.</summary>
public abstract class AuditableAggregateRoot : AggregateRoot, IAuditableEntity
{
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
