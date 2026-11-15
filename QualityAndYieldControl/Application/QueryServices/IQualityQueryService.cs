using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Queries;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ReadModels;

namespace Molinex.Platform.QualityAndYieldControl.Application.QueryServices;

public interface IQualityQueryService
{
    Task<QualityAssessment?> Handle(GetQualityAssessmentByIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<QualityAssessment>> Handle(GetQualityAssessmentsQuery query, CancellationToken cancellationToken);

    Task<WasteRecord?> Handle(GetWasteRecordByIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<WasteRecord>> Handle(GetWasteRecordsQuery query, CancellationToken cancellationToken);

    Task<Result<YieldIndicatorsReport>> Handle(GetYieldIndicatorsQuery query, CancellationToken cancellationToken);

    Task<Result<RiceCompositionReport>> Handle(GetRiceCompositionQuery query, CancellationToken cancellationToken);
}
