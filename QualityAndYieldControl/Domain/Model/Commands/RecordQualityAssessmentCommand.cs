using Cortex.Mediator.Commands;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.Commands;

public record QualityMeasurementData(string Indicator, decimal Value, decimal ExpectedMinimum, decimal ExpectedMaximum);

public record RecordQualityAssessmentCommand(
    long ProductionRecordId,
    IReadOnlyCollection<QualityMeasurementData> Measurements,
    DateTime AssessedAt) : ICommand<Result<QualityAssessment>>;
