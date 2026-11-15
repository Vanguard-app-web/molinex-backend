using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Commands;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Events;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;

public class MaintenanceRecord : AuditableAggregateRoot
{
    private MaintenanceRecord()
    {
        Description = null!;
        Responsible = null!;
    }

    public MaintenanceRecord(RecordMaintenanceCommand command)
    {
        Guard.Against(command.MachineId <= 0, MaintenanceErrors.MachineNotFound);
        MachineId = new MachineId(command.MachineId);
        Type = MaintenanceTypeExtensions.FromCode(command.Type);
        PerformedAt = Guard.UtcInstant(command.PerformedAt, MaintenanceErrors.InvalidMaintenanceDate);
        Description = new MaintenanceDescription(command.Description);
        Responsible = new TechnicianReference(command.TechnicianName, command.TechnicianId);
        AnomalyReference = string.IsNullOrWhiteSpace(command.AnomalyId) ? null : new AnomalyReference(command.AnomalyId);

        if (Type == MaintenanceType.Corrective)
        {
            Guard.Against(!command.DowntimeMinutes.HasValue, MaintenanceErrors.CorrectiveDetailsRequired);
            CorrectiveDetails = new CorrectiveMaintenanceDetails(command.Failure, command.Cause, command.ActionTaken,
                command.DowntimeMinutes!.Value);
        }
        else
        {
            var hasFailureDetails = !string.IsNullOrWhiteSpace(command.Failure)
                                    || !string.IsNullOrWhiteSpace(command.Cause)
                                    || !string.IsNullOrWhiteSpace(command.ActionTaken)
                                    || command.DowntimeMinutes.HasValue;
            Guard.Against(hasFailureDetails, MaintenanceErrors.PreventiveWithFailureDetails);
        }

        AddDomainEvent(() => new MaintenanceRecordedEvent(Id, MachineId, Type.ToCode()));
    }

    public MaintenanceRecordId Id { get; private set; }
    public MachineId MachineId { get; private set; }
    public MaintenanceType Type { get; private set; }
    public DateTime PerformedAt { get; private set; }
    public MaintenanceDescription Description { get; private set; }
    public TechnicianReference Responsible { get; private set; }
    public AnomalyReference? AnomalyReference { get; private set; }
    public CorrectiveMaintenanceDetails? CorrectiveDetails { get; private set; }
}
