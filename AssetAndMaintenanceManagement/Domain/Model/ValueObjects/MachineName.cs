using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

public sealed record MachineName
{
    public const int MaxLength = 100;

    public MachineName(string? value)
    {
        Value = Guard.RequiredText(value, MaxLength, MaintenanceErrors.InvalidMachineName);
    }

    public string Value { get; }
}
