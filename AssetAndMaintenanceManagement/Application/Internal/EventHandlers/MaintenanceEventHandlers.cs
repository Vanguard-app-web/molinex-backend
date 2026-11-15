using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Events;
using Molinex.Platform.Shared.Application.Internal.EventHandlers;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Application.Internal.EventHandlers;

public class MaintenanceEventHandlers(ILogger<MaintenanceEventHandlers> logger)
    : IEventHandler<MachineRegisteredEvent>, IEventHandler<MaintenanceRecordedEvent>
{
    public Task Handle(MachineRegisteredEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Machine {MachineId} ({MachineCode}) registered", domainEvent.MachineId.Value,
            domainEvent.MachineCode);
        return Task.CompletedTask;
    }

    public Task Handle(MaintenanceRecordedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("{Type} maintenance {RecordId} recorded for machine {MachineId}", domainEvent.Type,
            domainEvent.RecordId.Value, domainEvent.MachineId.Value);
        return Task.CompletedTask;
    }
}
