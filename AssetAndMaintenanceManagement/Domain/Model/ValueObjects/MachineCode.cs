using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

/// <summary>Machine code, always normalized to upper case.</summary>
public sealed record MachineCode
{
    public const int MaxLength = 30;

    public MachineCode(string? value)
    {
        Value = Guard.RequiredText(value, MaxLength, MaintenanceErrors.InvalidMachineCode).ToUpperInvariant();
    }

    public string Value { get; }
}
