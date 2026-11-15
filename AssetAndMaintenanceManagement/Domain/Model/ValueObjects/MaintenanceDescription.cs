using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

public sealed record MaintenanceDescription
{
    public const int MaxLength = 500;

    public MaintenanceDescription(string? value)
    {
        Value = Guard.RequiredText(value, MaxLength, MaintenanceErrors.InvalidDescription);
    }

    public string Value { get; }
}
