using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Repositories;

public interface IWasteRecordRepository : IBaseRepository<WasteRecord>
{
    Task<IEnumerable<WasteRecord>> ListAsync(ProductionRecordId? productionRecordId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WasteRecord>> ListByProductionRecordsAsync(
        IReadOnlyCollection<ProductionRecordId> productionRecordIds, CancellationToken cancellationToken = default);
}
