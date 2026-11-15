using Molinex.Platform.QualityAndYieldControl.Domain.Model.Commands;
using Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Resources;

namespace Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Transform;

public static class RecordQualityAssessmentCommandFromResourceAssembler
{
    public static RecordQualityAssessmentCommand ToCommandFromResource(CreateQualityAssessmentResource resource)
    {
        var measurements = (resource.Measurements ?? [])
            .Where(measurement => measurement is not null)
            .Select(measurement => new QualityMeasurementData(measurement.Indicator, measurement.Value,
                measurement.ExpectedMinimum, measurement.ExpectedMaximum))
            .ToList();
        return new RecordQualityAssessmentCommand(resource.ProductionRecordId, measurements, resource.AssessedAt);
    }
}

public static class RecordWasteCommandFromResourceAssembler
{
    public static RecordWasteCommand ToCommandFromResource(CreateWasteRecordResource resource) =>
        new(resource.ProductionRecordId, resource.QuantityValue, resource.QuantityUnit, resource.BaseWeightValue,
            resource.BaseWeightUnit, resource.RecordedAt);
}
