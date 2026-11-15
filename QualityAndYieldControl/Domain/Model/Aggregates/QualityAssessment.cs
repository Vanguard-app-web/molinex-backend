using Molinex.Platform.QualityAndYieldControl.Domain.Model.Commands;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Errors;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Events;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;

public class QualityAssessment : AuditableAggregateRoot
{
    private readonly List<QualityMeasurement> _measurements = [];

    private QualityAssessment()
    {
    }

    public QualityAssessment(RecordQualityAssessmentCommand command)
    {
        Guard.Against(command.ProductionRecordId <= 0, QualityErrors.ProductionRecordNotFound);
        ProductionRecordId = new ProductionRecordId(command.ProductionRecordId);
        AssessedAt = Guard.UtcInstant(command.AssessedAt, QualityErrors.InvalidAssessmentDate);
        Guard.Against(command.Measurements is null || command.Measurements.Count == 0,
            QualityErrors.MeasurementsRequired);
        foreach (var data in command.Measurements!)
        {
            Guard.Against(data is null, QualityErrors.MeasurementsRequired);
            RecordMeasurement(QualityMeasurement.From(data!.Indicator, data.Value, data.ExpectedMinimum,
                data.ExpectedMaximum));
        }

        Guard.Against(!HasValidGrainComposition(), QualityErrors.GrainCompositionExceeded);
        AddDomainEvent(() => new QualityAssessmentRecordedEvent(Id, ProductionRecordId));
    }

    public QualityAssessmentId Id { get; private set; }
    public ProductionRecordId ProductionRecordId { get; private set; }
    public DateTime AssessedAt { get; private set; }
    public IReadOnlyCollection<QualityMeasurement> Measurements => _measurements.AsReadOnly();

    public decimal? MeasurementValue(QualityIndicator indicator) =>
        _measurements.FirstOrDefault(measurement => measurement.Indicator == indicator)?.Value.Value;

    /// <summary>Whole grain plus broken grain can never exceed 100 %.</summary>
    public bool HasValidGrainComposition()
    {
        var wholeGrain = MeasurementValue(QualityIndicator.WholeGrainPercentage) ?? 0m;
        var brokenGrain = MeasurementValue(QualityIndicator.BrokenGrainPercentage) ?? 0m;
        return wholeGrain + brokenGrain <= Percentage.Maximum;
    }

    private void RecordMeasurement(QualityMeasurement measurement)
    {
        Guard.Against(_measurements.Any(existing => existing.Indicator == measurement.Indicator),
            QualityErrors.DuplicateIndicator);
        _measurements.Add(measurement);
    }
}
