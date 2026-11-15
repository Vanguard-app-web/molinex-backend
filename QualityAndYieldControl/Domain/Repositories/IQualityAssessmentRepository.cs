using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Repositories;

public interface IQualityAssessmentRepository : IBaseRepository<QualityAssessment>
{
    Task<IEnumerable<QualityAssessment>> ListAsync(ProductionRecordId? productionRecordId,
        CancellationToken cancellationToken = default);

    /// <summary>Assessments of the given production records, most recent first.</summary>
    Task<IReadOnlyList<QualityAssessment>> ListByProductionRecordsAsync(
        IReadOnlyCollection<ProductionRecordId> productionRecordIds, CancellationToken cancellationToken = default);
}
