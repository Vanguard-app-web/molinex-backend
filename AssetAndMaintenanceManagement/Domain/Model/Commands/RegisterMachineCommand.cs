using Cortex.Mediator.Commands;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Commands;

public record RegisterMachineCommand(
    string Code,
    string Name,
    string Model,
    string Status,
    DateTime? StatusChangedAt) : ICommand<Result<Machine>>;
