using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

public enum MaintenanceType
{
    Preventive,
    Corrective
}

public static class MaintenanceTypeExtensions
{
    public static string ToCode(this MaintenanceType type) =>
        type == MaintenanceType.Corrective ? "CORRECTIVE" : "PREVENTIVE";

    public static MaintenanceType FromCode(string? code)
    {
        return code?.Trim() switch
        {
            "PREVENTIVE" => MaintenanceType.Preventive,
            "CORRECTIVE" => MaintenanceType.Corrective,
            _ => throw new DomainRuleException(MaintenanceErrors.InvalidMaintenanceType)
        };
    }
}
