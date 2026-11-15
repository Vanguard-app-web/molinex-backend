using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.Queries;

namespace Molinex.Platform.ProductionManagement.Application.QueryServices;

public interface IProductionQueryService
{
    Task<IEnumerable<RawMaterialReception>> Handle(GetAllRawMaterialReceptionsQuery query,
        CancellationToken cancellationToken);

    Task<RawMaterialReception?> Handle(GetRawMaterialReceptionByIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<ProductionBatch>> Handle(GetProductionBatchesQuery query, CancellationToken cancellationToken);

    Task<ProductionBatch?> Handle(GetProductionBatchByIdQuery query, CancellationToken cancellationToken);

    Task<ProductionRecord?> Handle(GetProductionRecordByIdQuery query, CancellationToken cancellationToken);

    Task<Result<IEnumerable<ProductionRecord>>> Handle(GetProductionRecordsQuery query,
        CancellationToken cancellationToken);

    Task<Result<IEnumerable<ProductionRecord>>> Handle(GetProductionHistoryQuery query,
        CancellationToken cancellationToken);
}
