using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Commands;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Events;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;

public class Machine : AuditableAggregateRoot
{
    private Machine()
    {
        Code = null!;
        Name = null!;
        Model = null!;
    }

    public Machine(RegisterMachineCommand command)
    {
        Code = new MachineCode(command.Code);
        Name = new MachineName(command.Name);
        Model = new MachineModel(command.Model);
        Status = MachineStatusExtensions.FromCode(command.Status);
        Guard.Against(!command.StatusChangedAt.HasValue, MaintenanceErrors.StatusChangeDateRequired);
        StatusChangedAt = Guard.UtcInstant(command.StatusChangedAt!.Value, MaintenanceErrors.StatusChangeDateRequired);
        AddDomainEvent(() => new MachineRegisteredEvent(Id, Code.Value));
    }

    public MachineId Id { get; private set; }
    public MachineCode Code { get; private set; }
    public MachineName Name { get; private set; }
    public MachineModel Model { get; private set; }
    public MachineStatus Status { get; private set; }
    public DateTime StatusChangedAt { get; private set; }
}
