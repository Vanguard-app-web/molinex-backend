using Molinex.Platform.ProductionManagement.Application.QueryServices;
using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.Queries;
using Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;
using Molinex.Platform.ProductionManagement.Domain.Repositories;

namespace Molinex.Platform.ProductionManagement.Application.Internal.QueryServices;

public class ProductionQueryService(
    IRawMaterialReceptionRepository receptionRepository,
    IProductionBatchRepository batchRepository,
    IProductionRecordRepository recordRepository)
    : IProductionQueryService
{
    public async Task<IEnumerable<RawMaterialReception>> Handle(GetAllRawMaterialReceptionsQuery query,
        CancellationToken cancellationToken)
    {
        return await receptionRepository.ListAsync(cancellationToken);
    }

    public async Task<RawMaterialReception?> Handle(GetRawMaterialReceptionByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await receptionRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }

    public async Task<IEnumerable<ProductionBatch>> Handle(GetProductionBatchesQuery query,
        CancellationToken cancellationToken)
    {
        RawMaterialReceptionId? receptionId =
            query.ReceptionId.HasValue ? new RawMaterialReceptionId(query.ReceptionId.Value) : null;
        return await batchRepository.ListAsync(receptionId, cancellationToken);
    }

    public async Task<ProductionBatch?> Handle(GetProductionBatchByIdQuery query, CancellationToken cancellationToken)
    {
        return await batchRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }

    public async Task<ProductionRecord?> Handle(GetProductionRecordByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await recordRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }

    public Task<Result<IEnumerable<ProductionRecord>>> Handle(GetProductionRecordsQuery query,
        CancellationToken cancellationToken)
    {
        return ListRecords(query.BatchId, query.From, query.To, false, cancellationToken);
    }

    public Task<Result<IEnumerable<ProductionRecord>>> Handle(GetProductionHistoryQuery query,
        CancellationToken cancellationToken)
    {
        return ListRecords(null, query.From, query.To, true, cancellationToken);
    }

    private async Task<Result<IEnumerable<ProductionRecord>>> ListRecords(long? batchId, DateTime? from, DateTime? to,
        bool newestFirst, CancellationToken cancellationToken)
    {
        if (from > to) return Result<IEnumerable<ProductionRecord>>.Failure(SharedErrors.InvalidDateRange);
        ProductionBatchId? batch = batchId.HasValue ? new ProductionBatchId(batchId.Value) : null;
        var records = await recordRepository.ListAsync(batch, ToUtc(from), ToUtc(to), newestFirst, cancellationToken);
        return Result<IEnumerable<ProductionRecord>>.Success(records);
    }

    private static DateTime? ToUtc(DateTime? value) =>
        value.HasValue ? Guard.UtcInstant(value.Value, SharedErrors.InvalidDateRange) : null;
}
