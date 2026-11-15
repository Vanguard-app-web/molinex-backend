using Cortex.Mediator.Commands;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Commands;

public record RecordMaintenanceCommand(
    long MachineId,
    string Type,
    DateTime PerformedAt,
    string Description,
    string? TechnicianId,
    string TechnicianName,
    string? AnomalyId,
    string? Failure,
    string? Cause,
    string? ActionTaken,
    int? DowntimeMinutes) : ICommand<Result<MaintenanceRecord>>;
