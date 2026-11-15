using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Queries;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Application.QueryServices;

public interface IMaintenanceQueryService
{
    Task<IEnumerable<Machine>> Handle(GetMachinesQuery query, CancellationToken cancellationToken);
    Task<Machine?> Handle(GetMachineByIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<MaintenanceRecord>> Handle(GetMaintenanceRecordsQuery query, CancellationToken cancellationToken);

    Task<MaintenanceRecord?> Handle(GetMaintenanceRecordByIdQuery query, CancellationToken cancellationToken);

    Task<Result<IEnumerable<MaintenanceRecord>>> Handle(GetMaintenanceHistoryQuery query,
        CancellationToken cancellationToken);
}
