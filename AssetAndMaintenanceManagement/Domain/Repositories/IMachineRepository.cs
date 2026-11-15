using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Repositories;

public interface IMachineRepository : IBaseRepository<Machine>
{
    Task<bool> ExistsByCodeAsync(MachineCode code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Machine>> ListAsync(CancellationToken cancellationToken = default);
}
