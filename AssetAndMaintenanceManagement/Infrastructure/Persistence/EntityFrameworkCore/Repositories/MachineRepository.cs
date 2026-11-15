using Microsoft.EntityFrameworkCore;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Repositories;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class MachineRepository(AppDbContext context) : BaseRepository<Machine>(context), IMachineRepository
{
    public async Task<bool> ExistsByCodeAsync(MachineCode code, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Machine>().AsNoTracking().AnyAsync(machine => machine.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Machine>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Machine>().AsNoTracking().OrderBy(machine => machine.Id)
            .ToListAsync(cancellationToken);
    }
}
