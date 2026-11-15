namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.Queries;

public record GetQualityAssessmentByIdQuery(long Id);

public record GetQualityAssessmentsQuery(long? ProductionRecordId);

public record GetWasteRecordByIdQuery(long Id);

public record GetWasteRecordsQuery(long? ProductionRecordId);

public record GetYieldIndicatorsQuery(long? ProductionRecordId, DateTime? From, DateTime? To);

public record GetRiceCompositionQuery(long BatchId);
