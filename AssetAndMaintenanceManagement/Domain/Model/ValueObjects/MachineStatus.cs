using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

public enum MachineStatus
{
    Operational,
    RequiresAttention,
    UnderMaintenance,
    OutOfService
}

public static class MachineStatusExtensions
{
    public static string ToCode(this MachineStatus status)
    {
        return status switch
        {
            MachineStatus.Operational => "OPERATIONAL",
            MachineStatus.RequiresAttention => "REQUIRES_ATTENTION",
            MachineStatus.UnderMaintenance => "UNDER_MAINTENANCE",
            _ => "OUT_OF_SERVICE"
        };
    }

    public static MachineStatus FromCode(string? code)
    {
        return code?.Trim() switch
        {
            "OPERATIONAL" => MachineStatus.Operational,
            "REQUIRES_ATTENTION" => MachineStatus.RequiresAttention,
            "UNDER_MAINTENANCE" => MachineStatus.UnderMaintenance,
            "OUT_OF_SERVICE" => MachineStatus.OutOfService,
            _ => throw new DomainRuleException(MaintenanceErrors.InvalidMachineStatus)
        };
    }
}
