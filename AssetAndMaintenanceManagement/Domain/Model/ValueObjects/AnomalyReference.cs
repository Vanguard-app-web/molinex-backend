using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

/// <summary>Reference by identity to an anomaly attended by a maintenance record.</summary>
public sealed record AnomalyReference
{
    public const int MaxLength = 100;

    public AnomalyReference(string? anomalyId)
    {
        AnomalyId = Guard.RequiredText(anomalyId, MaxLength, MaintenanceErrors.InvalidAnomalyReference);
    }

    public string AnomalyId { get; }
}
