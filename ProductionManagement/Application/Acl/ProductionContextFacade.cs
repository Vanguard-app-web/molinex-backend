using Molinex.Platform.ProductionManagement.Application.QueryServices;
using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.Queries;
using Molinex.Platform.ProductionManagement.Interfaces.Acl;

namespace Molinex.Platform.ProductionManagement.Application.Acl;

/// <summary>
///     Only entry point other bounded contexts may use to read Production Management.
///     It delegates to the context's own query service and returns read contracts built from primitives.
/// </summary>
public class ProductionContextFacade(IProductionQueryService queryService) : IProductionContextFacade
{
    public async Task<bool> ExistsProductionRecordById(long productionRecordId, CancellationToken cancellationToken)
    {
        if (productionRecordId <= 0) return false;
        var record = await queryService.Handle(new GetProductionRecordByIdQuery(productionRecordId), cancellationToken);
        return record is not null;
    }

    public async Task<bool> ExistsProductionBatchById(long productionBatchId, CancellationToken cancellationToken)
    {
        if (productionBatchId <= 0) return false;
        var batch = await queryService.Handle(new GetProductionBatchByIdQuery(productionBatchId), cancellationToken);
        return batch is not null;
    }

    public async Task<ProductionRecordSummary?> FetchProductionRecordById(long productionRecordId,
        CancellationToken cancellationToken)
    {
        if (productionRecordId <= 0) return null;
        var record = await queryService.Handle(new GetProductionRecordByIdQuery(productionRecordId), cancellationToken);
        return record is null ? null : ToSummary(record);
    }

    public async Task<IReadOnlyList<ProductionRecordSummary>> FetchProductionRecords(long? productionBatchId,
        DateTime? from, DateTime? to, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetProductionRecordsQuery(productionBatchId, from, to),
            cancellationToken);
        return result.IsSuccess ? result.Value!.Select(ToSummary).ToList() : [];
    }

    private static ProductionRecordSummary ToSummary(ProductionRecord record)
    {
        return new ProductionRecordSummary(
            record.Id.Value,
            record.BatchId.Value,
            record.Details.ProcessedWeight.Value,
            record.Details.ProcessedWeight.Unit.ToCode(),
            record.Details.Period.StartedAt);
    }
}
