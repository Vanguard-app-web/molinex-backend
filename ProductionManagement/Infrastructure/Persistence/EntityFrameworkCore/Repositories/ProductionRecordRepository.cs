using Microsoft.EntityFrameworkCore;
using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;
using Molinex.Platform.ProductionManagement.Domain.Repositories;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Molinex.Platform.ProductionManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ProductionRecordRepository(AppDbContext context)
    : BaseRepository<ProductionRecord>(context), IProductionRecordRepository
{
    public async Task<IEnumerable<ProductionRecord>> ListAsync(ProductionBatchId? batchId, DateTime? startedFrom,
        DateTime? startedTo, bool newestFirst, CancellationToken cancellationToken = default)
    {
        var records = Context.Set<ProductionRecord>().AsNoTracking();
        if (batchId.HasValue) records = records.Where(record => record.BatchId == batchId.Value);
        if (startedFrom.HasValue) records = records.Where(record => record.Details.Period.StartedAt >= startedFrom.Value);
        if (startedTo.HasValue) records = records.Where(record => record.Details.Period.StartedAt <= startedTo.Value);
        records = newestFirst
            ? records.OrderByDescending(record => record.Details.Period.StartedAt).ThenByDescending(record => record.Id)
            : records.OrderBy(record => record.Id);
        return await records.ToListAsync(cancellationToken);
    }
}
