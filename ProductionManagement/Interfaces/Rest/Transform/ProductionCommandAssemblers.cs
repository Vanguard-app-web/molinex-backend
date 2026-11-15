using Molinex.Platform.ProductionManagement.Domain.Model.Commands;
using Molinex.Platform.ProductionManagement.Interfaces.Rest.Resources;

namespace Molinex.Platform.ProductionManagement.Interfaces.Rest.Transform;

public static class RecordRawMaterialReceptionCommandFromResourceAssembler
{
    public static RecordRawMaterialReceptionCommand ToCommandFromResource(CreateRawMaterialReceptionResource resource) =>
        new(resource.ReceivedAt, resource.Supplier, resource.Origin, resource.Quantity, resource.Unit);
}

public static class RegisterProductionBatchCommandFromResourceAssembler
{
    public static RegisterProductionBatchCommand ToCommandFromResource(CreateProductionBatchResource resource) =>
        new(resource.Code, resource.ReceptionId, resource.RegisteredAt);
}

public static class RecordProductionInformationCommandFromResourceAssembler
{
    public static RecordProductionInformationCommand ToCommandFromResource(CreateProductionRecordResource resource) =>
        new(resource.BatchId, resource.ProcessName, resource.ProcessedWeight, resource.Unit, resource.StartedAt,
            resource.FinishedAt, resource.Status);
}

public static class UpdateProductionInformationCommandFromResourceAssembler
{
    public static UpdateProductionInformationCommand ToCommandFromResource(long recordId,
        UpdateProductionRecordResource resource) =>
        new(recordId, resource.BatchId, resource.ProcessName, resource.ProcessedWeight, resource.Unit,
            resource.StartedAt, resource.FinishedAt, resource.Status);
}
