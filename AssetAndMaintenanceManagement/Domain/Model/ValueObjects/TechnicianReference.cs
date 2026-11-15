using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

/// <summary>Who performed the maintenance: a display name and, optionally, the identity of the principal.</summary>
public sealed record TechnicianReference
{
    public const int MaxLength = 100;

    public TechnicianReference(string? displayName, string? principalId)
    {
        DisplayName = Guard.RequiredText(displayName, MaxLength, MaintenanceErrors.InvalidTechnician);
        PrincipalId = Guard.OptionalText(principalId, MaxLength, MaintenanceErrors.InvalidTechnician);
    }

    public string DisplayName { get; }
    public string? PrincipalId { get; }
}
