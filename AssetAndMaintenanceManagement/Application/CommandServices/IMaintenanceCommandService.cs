using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Commands;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Application.CommandServices;

/// <summary>Application contract for Asset and Maintenance commands, dispatched through the mediator.</summary>
public interface IMaintenanceCommandService
{
    Task<Result<Machine>> Handle(RegisterMachineCommand command, CancellationToken cancellationToken);
    Task<Result<MaintenanceRecord>> Handle(RecordMaintenanceCommand command, CancellationToken cancellationToken);
}
