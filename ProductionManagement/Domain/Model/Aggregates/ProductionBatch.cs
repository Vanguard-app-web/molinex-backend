using Molinex.Platform.ProductionManagement.Domain.Model.Commands;
using Molinex.Platform.ProductionManagement.Domain.Model.Errors;
using Molinex.Platform.ProductionManagement.Domain.Model.Events;
using Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;

public class ProductionBatch : AuditableAggregateRoot
{
    private ProductionBatch()
    {
        Code = null!;
    }

    public ProductionBatch(RegisterProductionBatchCommand command)
    {
        Code = new BatchCode(command.Code);
        Guard.Against(command.ReceptionId <= 0, ProductionErrors.ReceptionNotFound);
        ReceptionId = new RawMaterialReceptionId(command.ReceptionId);
        RegisteredAt = Guard.UtcInstant(command.RegisteredAt, ProductionErrors.InvalidRegistrationDate);
        AddDomainEvent(() => new ProductionBatchRegisteredEvent(Id, Code.Value, ReceptionId));
    }

    public ProductionBatchId Id { get; private set; }
    public BatchCode Code { get; private set; }
    public RawMaterialReceptionId ReceptionId { get; private set; }
    public DateTime RegisteredAt { get; private set; }
}
