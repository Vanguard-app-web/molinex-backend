using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.ProductionManagement.Domain.Repositories;

public interface IProductionRecordRepository : IBaseRepository<ProductionRecord>
{
    /// <param name="newestFirst">Orders by start date descending instead of by identifier.</param>
    Task<IEnumerable<ProductionRecord>> ListAsync(ProductionBatchId? batchId, DateTime? startedFrom,
        DateTime? startedTo, bool newestFirst, CancellationToken cancellationToken = default);
}
