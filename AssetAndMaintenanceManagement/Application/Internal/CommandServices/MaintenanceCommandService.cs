using Cortex.Mediator.Commands;
using Molinex.Platform.AssetAndMaintenanceManagement.Application.CommandServices;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Commands;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Repositories;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Application.Internal.CommandServices;

public class MaintenanceCommandService(
    IMachineRepository machineRepository,
    IMaintenanceRecordRepository maintenanceRepository)
    : IMaintenanceCommandService,
        ICommandHandler<RegisterMachineCommand, Result<Machine>>,
        ICommandHandler<RecordMaintenanceCommand, Result<MaintenanceRecord>>
{
    public async Task<Result<Machine>> Handle(RegisterMachineCommand command, CancellationToken cancellationToken)
    {
        Machine machine;
        try
        {
            machine = new Machine(command);
        }
        catch (DomainRuleException exception)
        {
            return Result<Machine>.Failure(exception.Error);
        }

        if (await machineRepository.ExistsByCodeAsync(machine.Code, cancellationToken))
            return Result<Machine>.Failure(MaintenanceErrors.DuplicateMachineCode);

        await machineRepository.AddAsync(machine, cancellationToken);
        return Result<Machine>.Success(machine);
    }

    public async Task<Result<MaintenanceRecord>> Handle(RecordMaintenanceCommand command,
        CancellationToken cancellationToken)
    {
        MaintenanceRecord record;
        try
        {
            record = new MaintenanceRecord(command);
        }
        catch (DomainRuleException exception)
        {
            return Result<MaintenanceRecord>.Failure(exception.Error);
        }

        if (!await machineRepository.ExistsByIdAsync(record.MachineId.Value, cancellationToken))
            return Result<MaintenanceRecord>.Failure(MaintenanceErrors.MachineNotFound);

        await maintenanceRepository.AddAsync(record, cancellationToken);
        return Result<MaintenanceRecord>.Success(record);
    }
}
