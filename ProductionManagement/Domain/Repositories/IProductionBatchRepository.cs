using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.ProductionManagement.Domain.Repositories;

public interface IProductionBatchRepository : IBaseRepository<ProductionBatch>
{
    Task<bool> ExistsByCodeAsync(BatchCode code, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductionBatch>> ListAsync(RawMaterialReceptionId? receptionId,
        CancellationToken cancellationToken = default);
}
