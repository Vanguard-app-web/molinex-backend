namespace Molinex.Platform.ProductionManagement.Interfaces.Acl;

/// <summary>Published contract of Production Management for other bounded contexts.</summary>
public interface IProductionContextFacade
{
    Task<bool> ExistsProductionRecordById(long productionRecordId, CancellationToken cancellationToken);

    Task<bool> ExistsProductionBatchById(long productionBatchId, CancellationToken cancellationToken);

    /// <returns>The production record summary, or <c>null</c> when it does not exist.</returns>
    Task<ProductionRecordSummary?> FetchProductionRecordById(long productionRecordId,
        CancellationToken cancellationToken);

    /// <summary>Production records narrowed by batch and by the period in which they started.</summary>
    Task<IReadOnlyList<ProductionRecordSummary>> FetchProductionRecords(long? productionBatchId, DateTime? from,
        DateTime? to, CancellationToken cancellationToken);
}
