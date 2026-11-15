using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

public sealed record MachineModel
{
    public const int MaxLength = 100;

    public MachineModel(string? value)
    {
        Value = Guard.RequiredText(value, MaxLength, MaintenanceErrors.InvalidMachineModel);
    }

    public string Value { get; }
}
