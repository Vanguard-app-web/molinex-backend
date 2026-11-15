using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

namespace Molinex.Platform.QualityAndYieldControl.Application.OutboundServices.Acl;

/// <summary>
///     Outbound port through which Quality and Yield Control reads Production Management.
///     It speaks this context's own model only.
/// </summary>
public interface IExternalProductionService
{
    Task<bool> ExistsProductionRecord(ProductionRecordId productionRecordId, CancellationToken cancellationToken);

    Task<bool> ExistsProductionBatch(long productionBatchId, CancellationToken cancellationToken);

    Task<ProductionRecordReference?> FetchProductionRecord(ProductionRecordId productionRecordId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductionRecordReference>> FetchProductionRecords(long? productionBatchId, DateTime? from,
        DateTime? to, CancellationToken cancellationToken);
}
