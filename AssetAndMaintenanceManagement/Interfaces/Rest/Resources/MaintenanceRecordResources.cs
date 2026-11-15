using System.Text.Json.Serialization;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Interfaces.Rest.Resources;

public record CreateMaintenanceRecordResource(
    [property: JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
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
    int? DowntimeMinutes);

public record MaintenanceRecordResource(
    string Id,
    string MachineId,
    string Type,
    DateTime PerformedAt,
    string Description,
    string? TechnicianId,
    string TechnicianName,
    string? AnomalyId,
    string? Failure,
    string? Cause,
    string? ActionTaken,
    int? DowntimeMinutes);
