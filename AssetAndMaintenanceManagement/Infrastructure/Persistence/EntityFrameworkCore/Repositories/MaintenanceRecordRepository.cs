using Microsoft.EntityFrameworkCore;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Repositories;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class MaintenanceRecordRepository(AppDbContext context)
    : BaseRepository<MaintenanceRecord>(context), IMaintenanceRecordRepository
{
    public async Task<IEnumerable<MaintenanceRecord>> ListAsync(MachineId? machineId, bool newestFirst,
        CancellationToken cancellationToken = default)
    {
        var records = Context.Set<MaintenanceRecord>().AsNoTracking();
        if (machineId.HasValue) records = records.Where(record => record.MachineId == machineId.Value);
        records = newestFirst
            ? records.OrderByDescending(record => record.PerformedAt).ThenByDescending(record => record.Id)
            : records.OrderBy(record => record.Id);
        return await records.ToListAsync(cancellationToken);
    }
}
