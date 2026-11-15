using Microsoft.EntityFrameworkCore;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;
using Molinex.Platform.QualityAndYieldControl.Domain.Repositories;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Molinex.Platform.QualityAndYieldControl.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class WasteRecordRepository(AppDbContext context)
    : BaseRepository<WasteRecord>(context), IWasteRecordRepository
{
    public async Task<IEnumerable<WasteRecord>> ListAsync(ProductionRecordId? productionRecordId,
        CancellationToken cancellationToken = default)
    {
        var wasteRecords = Context.Set<WasteRecord>().AsNoTracking();
        if (productionRecordId.HasValue)
            wasteRecords = wasteRecords.Where(waste => waste.ProductionRecordId == productionRecordId.Value);
        return await wasteRecords.OrderBy(waste => waste.Id).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WasteRecord>> ListByProductionRecordsAsync(
        IReadOnlyCollection<ProductionRecordId> productionRecordIds, CancellationToken cancellationToken = default)
    {
        if (productionRecordIds.Count == 0) return [];
        return await Context.Set<WasteRecord>().AsNoTracking()
            .Where(waste => productionRecordIds.Contains(waste.ProductionRecordId))
            .OrderBy(waste => waste.Id)
            .ToListAsync(cancellationToken);
    }
}
