using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

/// <summary>What failed, why, what was done and how long the machine was stopped.</summary>
public sealed record CorrectiveMaintenanceDetails
{
    public const int FailureMaxLength = 250;
    public const int CauseMaxLength = 250;
    public const int ActionTakenMaxLength = 500;

    public CorrectiveMaintenanceDetails(string? failure, string? cause, string? actionTaken, int downtimeMinutes)
    {
        Failure = Guard.RequiredText(failure, FailureMaxLength, MaintenanceErrors.CorrectiveDetailsRequired);
        Cause = Guard.RequiredText(cause, CauseMaxLength, MaintenanceErrors.CorrectiveDetailsRequired);
        ActionTaken = Guard.RequiredText(actionTaken, ActionTakenMaxLength, MaintenanceErrors.CorrectiveDetailsRequired);
        Guard.Against(downtimeMinutes < 0, MaintenanceErrors.InvalidDowntime);
        DowntimeMinutes = downtimeMinutes;
    }

    public string Failure { get; }
    public string Cause { get; }
    public string ActionTaken { get; }
    public int DowntimeMinutes { get; }
}
