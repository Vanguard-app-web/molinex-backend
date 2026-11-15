using Molinex.Platform.ProductionManagement.Interfaces.Acl;
using Molinex.Platform.QualityAndYieldControl.Application.OutboundServices.Acl;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

namespace Molinex.Platform.QualityAndYieldControl.Application.Internal.OutboundServices.Acl;

/// <summary>
///     Anti-corruption layer towards Production Management. It depends only on the published facade
///     contract and translates its read contract into this context's value objects.
/// </summary>
public class ExternalProductionService(IProductionContextFacade productionContextFacade) : IExternalProductionService
{
    public Task<bool> ExistsProductionRecord(ProductionRecordId productionRecordId,
        CancellationToken cancellationToken)
    {
        return productionContextFacade.ExistsProductionRecordById(productionRecordId.Value, cancellationToken);
    }

    public Task<bool> ExistsProductionBatch(long productionBatchId, CancellationToken cancellationToken)
    {
        return productionContextFacade.ExistsProductionBatchById(productionBatchId, cancellationToken);
    }

    public async Task<ProductionRecordReference?> FetchProductionRecord(ProductionRecordId productionRecordId,
        CancellationToken cancellationToken)
    {
        var summary = await productionContextFacade.FetchProductionRecordById(productionRecordId.Value,
            cancellationToken);
        return summary is null ? null : ToReference(summary);
    }

    public async Task<IReadOnlyList<ProductionRecordReference>> FetchProductionRecords(long? productionBatchId,
        DateTime? from, DateTime? to, CancellationToken cancellationToken)
    {
        var summaries = await productionContextFacade.FetchProductionRecords(productionBatchId, from, to,
            cancellationToken);
        return summaries.Select(ToReference).ToList();
    }

    private static ProductionRecordReference ToReference(ProductionRecordSummary summary)
    {
        return new ProductionRecordReference(
            new ProductionRecordId(summary.ProductionRecordId),
            summary.ProductionBatchId,
            Weight.From(summary.ProcessedWeightValue, summary.ProcessedWeightUnit),
            summary.StartedAt);
    }
}
