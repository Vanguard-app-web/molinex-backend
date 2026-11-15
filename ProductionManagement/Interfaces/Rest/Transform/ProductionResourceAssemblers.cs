using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;
using Molinex.Platform.ProductionManagement.Interfaces.Rest.Resources;

namespace Molinex.Platform.ProductionManagement.Interfaces.Rest.Transform;

public static class RawMaterialReceptionResourceFromEntityAssembler
{
    public static RawMaterialReceptionResource ToResourceFromEntity(RawMaterialReception entity) =>
        new(entity.Id.ToString(), entity.ReceivedAt, entity.Supplier.Name, entity.Origin.Description,
            entity.Quantity.Value, entity.Quantity.Unit.ToCode());
}

public static class ProductionBatchResourceFromEntityAssembler
{
    public static ProductionBatchResource ToResourceFromEntity(ProductionBatch entity) =>
        new(entity.Id.ToString(), entity.Code.Value, entity.ReceptionId.ToString(), entity.RegisteredAt);
}

public static class ProductionRecordResourceFromEntityAssembler
{
    public static ProductionRecordResource ToResourceFromEntity(ProductionRecord entity) =>
        new(entity.Id.ToString(), entity.BatchId.ToString(), entity.Details.ProcessName.Value,
            entity.Details.ProcessedWeight.Value, entity.Details.ProcessedWeight.Unit.ToCode(),
            entity.Details.Period.StartedAt, entity.Details.Period.FinishedAt, entity.Details.Status.ToCode());
}
