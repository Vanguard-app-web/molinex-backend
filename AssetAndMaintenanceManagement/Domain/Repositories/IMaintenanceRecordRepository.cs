using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Repositories;

public interface IMaintenanceRecordRepository : IBaseRepository<MaintenanceRecord>
{
    /// <param name="newestFirst">Orders by maintenance date descending instead of by identifier.</param>
    Task<IEnumerable<MaintenanceRecord>> ListAsync(MachineId? machineId, bool newestFirst,
        CancellationToken cancellationToken = default);
}
