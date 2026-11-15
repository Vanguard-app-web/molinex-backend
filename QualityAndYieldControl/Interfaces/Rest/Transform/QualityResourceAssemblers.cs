using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ReadModels;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;
using Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Resources;

namespace Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Transform;

public static class QualityAssessmentResourceFromEntityAssembler
{
    public static QualityAssessmentResource ToResourceFromEntity(QualityAssessment entity)
    {
        var measurements = entity.Measurements.Select(measurement => new QualityMeasurementResource(
            measurement.Indicator.ToCode(), measurement.Value.Value, measurement.ExpectedRange.Minimum.Value,
            measurement.ExpectedRange.Maximum.Value));
        return new QualityAssessmentResource(entity.Id.ToString(), entity.ProductionRecordId.ToString(),
            measurements.ToList(), entity.AssessedAt);
    }
}

public static class WasteRecordResourceFromEntityAssembler
{
    public static WasteRecordResource ToResourceFromEntity(WasteRecord entity) =>
        new(entity.Id.ToString(), entity.ProductionRecordId.ToString(), entity.Quantity.Value,
            entity.Quantity.Unit.ToCode(), entity.BaseWeight?.Value, entity.BaseWeight?.Unit.ToCode(),
            entity.Percentage?.Value, entity.RecordedAt);
}

public static class YieldIndicatorsResourceFromReportAssembler
{
    public static YieldIndicatorsResource ToResourceFromReport(YieldIndicatorsReport report)
    {
        var results = report.Indicators.Select(indicator => new YieldIndicatorResource(
            indicator.ProductionRecordId.ToString(), indicator.YieldPercentage, indicator.Available,
            indicator.WastePercentages));
        return new YieldIndicatorsResource(report.From, report.To, "%", results.ToList());
    }
}

public static class RiceCompositionResourceFromReportAssembler
{
    public static RiceCompositionResource ToResourceFromReport(RiceCompositionReport report)
    {
        var results = report.Compositions.Select(composition => new RiceCompositionItemResource(
            composition.ProductionRecordId.ToString(), composition.WholeGrainPercentage,
            composition.BrokenGrainPercentage, composition.AssessedAt));
        return new RiceCompositionResource(report.BatchId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            report.Available, results.ToList());
    }
}
