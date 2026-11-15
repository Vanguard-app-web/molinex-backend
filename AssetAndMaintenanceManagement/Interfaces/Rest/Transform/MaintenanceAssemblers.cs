using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Commands;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;
using Molinex.Platform.AssetAndMaintenanceManagement.Interfaces.Rest.Resources;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Interfaces.Rest.Transform;

public static class RegisterMachineCommandFromResourceAssembler
{
    public static RegisterMachineCommand ToCommandFromResource(CreateMachineResource resource) =>
        new(resource.Code, resource.Name, resource.Model, resource.Status, resource.StatusChangedAt);
}

public static class RecordMaintenanceCommandFromResourceAssembler
{
    public static RecordMaintenanceCommand ToCommandFromResource(CreateMaintenanceRecordResource resource) =>
        new(resource.MachineId, resource.Type, resource.PerformedAt, resource.Description, resource.TechnicianId,
            resource.TechnicianName, resource.AnomalyId, resource.Failure, resource.Cause, resource.ActionTaken,
            resource.DowntimeMinutes);
}

public static class MachineResourceFromEntityAssembler
{
    public static MachineResource ToResourceFromEntity(Machine entity) =>
        new(entity.Id.ToString(), entity.Code.Value, entity.Name.Value, entity.Model.Value, entity.Status.ToCode(),
            entity.StatusChangedAt);

    public static MachineStatusResource ToStatusResourceFromEntity(Machine entity) =>
        new(entity.Id.ToString(), entity.Status.ToCode(), entity.StatusChangedAt, false);
}

public static class MaintenanceRecordResourceFromEntityAssembler
{
    public static MaintenanceRecordResource ToResourceFromEntity(MaintenanceRecord entity) =>
        new(entity.Id.ToString(), entity.MachineId.ToString(), entity.Type.ToCode(), entity.PerformedAt,
            entity.Description.Value, entity.Responsible.PrincipalId, entity.Responsible.DisplayName,
            entity.AnomalyReference?.AnomalyId, entity.CorrectiveDetails?.Failure, entity.CorrectiveDetails?.Cause,
            entity.CorrectiveDetails?.ActionTaken, entity.CorrectiveDetails?.DowntimeMinutes);
}
