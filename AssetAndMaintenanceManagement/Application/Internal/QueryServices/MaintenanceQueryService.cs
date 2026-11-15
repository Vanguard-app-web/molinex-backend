using Molinex.Platform.AssetAndMaintenanceManagement.Application.QueryServices;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Queries;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Repositories;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Application.Internal.QueryServices;

public class MaintenanceQueryService(
    IMachineRepository machineRepository,
    IMaintenanceRecordRepository maintenanceRepository)
    : IMaintenanceQueryService
{
    public async Task<IEnumerable<Machine>> Handle(GetMachinesQuery query, CancellationToken cancellationToken)
    {
        return await machineRepository.ListAsync(cancellationToken);
    }

    public async Task<Machine?> Handle(GetMachineByIdQuery query, CancellationToken cancellationToken)
    {
        return await machineRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceRecord>> Handle(GetMaintenanceRecordsQuery query,
        CancellationToken cancellationToken)
    {
        MachineId? machineId = query.MachineId.HasValue ? new MachineId(query.MachineId.Value) : null;
        return await maintenanceRepository.ListAsync(machineId, false, cancellationToken);
    }

    public async Task<MaintenanceRecord?> Handle(GetMaintenanceRecordByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await maintenanceRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }

    public async Task<Result<IEnumerable<MaintenanceRecord>>> Handle(GetMaintenanceHistoryQuery query,
        CancellationToken cancellationToken)
    {
        if (!await machineRepository.ExistsByIdAsync(query.MachineId, cancellationToken))
            return Result<IEnumerable<MaintenanceRecord>>.Failure(MaintenanceErrors.MachineNotFound);
        var records = await maintenanceRepository.ListAsync(new MachineId(query.MachineId), true, cancellationToken);
        return Result<IEnumerable<MaintenanceRecord>>.Success(records);
    }
}
