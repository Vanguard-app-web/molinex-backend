using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;

namespace Molinex.Platform.ProductionManagement.Domain.Repositories;

public interface IRawMaterialReceptionRepository : IBaseRepository<RawMaterialReception>
{
    Task<IEnumerable<RawMaterialReception>> ListAsync(CancellationToken cancellationToken = default);
}
