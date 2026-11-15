using Microsoft.EntityFrameworkCore;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;
using Molinex.Platform.QualityAndYieldControl.Domain.Repositories;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Molinex.Platform.QualityAndYieldControl.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class QualityAssessmentRepository(AppDbContext context)
    : BaseRepository<QualityAssessment>(context), IQualityAssessmentRepository
{
    public async Task<IEnumerable<QualityAssessment>> ListAsync(ProductionRecordId? productionRecordId,
        CancellationToken cancellationToken = default)
    {
        var assessments = Context.Set<QualityAssessment>().AsNoTracking();
        if (productionRecordId.HasValue)
            assessments = assessments.Where(assessment => assessment.ProductionRecordId == productionRecordId.Value);
        return await assessments.OrderBy(assessment => assessment.Id).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<QualityAssessment>> ListByProductionRecordsAsync(
        IReadOnlyCollection<ProductionRecordId> productionRecordIds, CancellationToken cancellationToken = default)
    {
        if (productionRecordIds.Count == 0) return [];
        return await Context.Set<QualityAssessment>().AsNoTracking()
            .Where(assessment => productionRecordIds.Contains(assessment.ProductionRecordId))
            .OrderByDescending(assessment => assessment.AssessedAt).ThenByDescending(assessment => assessment.Id)
            .ToListAsync(cancellationToken);
    }
}
