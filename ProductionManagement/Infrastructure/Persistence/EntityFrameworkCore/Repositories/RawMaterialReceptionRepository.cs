using Microsoft.EntityFrameworkCore;
using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Repositories;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Molinex.Platform.ProductionManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class RawMaterialReceptionRepository(AppDbContext context)
    : BaseRepository<RawMaterialReception>(context), IRawMaterialReceptionRepository
{
    public async Task<IEnumerable<RawMaterialReception>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<RawMaterialReception>().AsNoTracking()
            .OrderBy(reception => reception.Id).ToListAsync(cancellationToken);
    }
}
