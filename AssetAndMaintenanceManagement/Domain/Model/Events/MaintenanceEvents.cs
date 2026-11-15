using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Events;

public record MachineRegisteredEvent(MachineId MachineId, string MachineCode) : IEvent;

public record MaintenanceRecordedEvent(MaintenanceRecordId RecordId, MachineId MachineId, string Type) : IEvent;
