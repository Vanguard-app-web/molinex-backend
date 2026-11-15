using System.Text.Json.Serialization;

namespace Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Resources;

public record QualityMeasurementResource(
    string Indicator,
    decimal Value,
    decimal ExpectedMinimum,
    decimal ExpectedMaximum);

public record CreateQualityAssessmentResource(
    [property: JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    long ProductionRecordId,
    List<QualityMeasurementResource> Measurements,
    DateTime AssessedAt);

public record QualityAssessmentResource(
    string Id,
    string ProductionRecordId,
    IEnumerable<QualityMeasurementResource> Measurements,
    DateTime AssessedAt);
