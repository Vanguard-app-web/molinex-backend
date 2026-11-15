using Microsoft.EntityFrameworkCore;
using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;
using Molinex.Platform.ProductionManagement.Domain.Repositories;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Molinex.Platform.ProductionManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ProductionBatchRepository(AppDbContext context)
    : BaseRepository<ProductionBatch>(context), IProductionBatchRepository
{
    /// <remarks>The code column uses a case-insensitive collation, so the comparison ignores letter case.</remarks>
    public async Task<bool> ExistsByCodeAsync(BatchCode code, CancellationToken cancellationToken = default)
    {
        return await Context.Set<ProductionBatch>().AsNoTracking()
            .AnyAsync(batch => batch.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<ProductionBatch>> ListAsync(RawMaterialReceptionId? receptionId,
        CancellationToken cancellationToken = default)
    {
        var batches = Context.Set<ProductionBatch>().AsNoTracking();
        if (receptionId.HasValue) batches = batches.Where(batch => batch.ReceptionId == receptionId.Value);
        return await batches.OrderBy(batch => batch.Id).ToListAsync(cancellationToken);
    }
}
