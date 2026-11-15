using Molinex.Platform.ProductionManagement.Domain.Model.Commands;
using Molinex.Platform.ProductionManagement.Domain.Model.Errors;
using Molinex.Platform.ProductionManagement.Domain.Model.Events;
using Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;

public class RawMaterialReception : AuditableAggregateRoot
{
    /// <summary>Tolerance for clock differences between the client and the server.</summary>
    public static readonly TimeSpan ClockTolerance = TimeSpan.FromMinutes(5);

    private RawMaterialReception()
    {
        Supplier = null!;
        Origin = null!;
        Quantity = null!;
    }

    public RawMaterialReception(RecordRawMaterialReceptionCommand command, DateTime utcNow)
    {
        ReceivedAt = Guard.UtcInstant(command.ReceivedAt, ProductionErrors.InvalidReceptionDate);
        Guard.Against(ReceivedAt > utcNow + ClockTolerance, ProductionErrors.InvalidReceptionDate);
        Supplier = new Supplier(command.Supplier);
        Origin = new MaterialOrigin(command.Origin);
        Quantity = Weight.From(command.Quantity, command.Unit);
        AddDomainEvent(() => new RawMaterialReceptionRecordedEvent(Id, ReceivedAt));
    }

    public RawMaterialReceptionId Id { get; private set; }
    public DateTime ReceivedAt { get; private set; }
    public Supplier Supplier { get; private set; }
    public MaterialOrigin Origin { get; private set; }
    public Weight Quantity { get; private set; }
}
