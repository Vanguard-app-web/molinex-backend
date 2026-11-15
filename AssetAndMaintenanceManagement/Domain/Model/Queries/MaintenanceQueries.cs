namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Queries;

public record GetMachinesQuery;

public record GetMachineByIdQuery(long Id);

public record GetMaintenanceRecordsQuery(long? MachineId);

public record GetMaintenanceRecordByIdQuery(long Id);

/// <summary>Maintenance history of one machine, most recent first.</summary>
public record GetMaintenanceHistoryQuery(long MachineId);
