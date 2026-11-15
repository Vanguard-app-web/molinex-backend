namespace Molinex.Platform.AssetAndMaintenanceManagement.Interfaces.Rest.Resources;

public record CreateMachineResource(string Code, string Name, string Model, string Status, DateTime? StatusChangedAt);

public record MachineResource(
    string Id,
    string Code,
    string Name,
    string Model,
    string Status,
    DateTime StatusChangedAt);

/// <param name="Pending">Always <c>false</c>: every machine is registered with the date of its status.</param>
public record MachineStatusResource(string Id, string Status, DateTime StatusChangedAt, bool Pending);
