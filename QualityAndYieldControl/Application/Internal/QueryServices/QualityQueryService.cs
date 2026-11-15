using Molinex.Platform.QualityAndYieldControl.Application.OutboundServices.Acl;
using Molinex.Platform.QualityAndYieldControl.Application.QueryServices;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Errors;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Queries;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ReadModels;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;
using Molinex.Platform.QualityAndYieldControl.Domain.Repositories;

namespace Molinex.Platform.QualityAndYieldControl.Application.Internal.QueryServices;

public class QualityQueryService(
    IQualityAssessmentRepository assessmentRepository,
    IWasteRecordRepository wasteRepository,
    IExternalProductionService externalProductionService)
    : IQualityQueryService
{
    public async Task<QualityAssessment?> Handle(GetQualityAssessmentByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await assessmentRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }

    public async Task<IEnumerable<QualityAssessment>> Handle(GetQualityAssessmentsQuery query,
        CancellationToken cancellationToken)
    {
        return await assessmentRepository.ListAsync(ToRecordId(query.ProductionRecordId), cancellationToken);
    }

    public async Task<WasteRecord?> Handle(GetWasteRecordByIdQuery query, CancellationToken cancellationToken)
    {
        return await wasteRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }

    public async Task<IEnumerable<WasteRecord>> Handle(GetWasteRecordsQuery query, CancellationToken cancellationToken)
    {
        return await wasteRepository.ListAsync(ToRecordId(query.ProductionRecordId), cancellationToken);
    }

    public async Task<Result<YieldIndicatorsReport>> Handle(GetYieldIndicatorsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.From > query.To) return Result<YieldIndicatorsReport>.Failure(SharedErrors.InvalidDateRange);

        IReadOnlyList<ProductionRecordReference> records;
        if (query.ProductionRecordId.HasValue)
        {
            var record = await externalProductionService.FetchProductionRecord(
                new ProductionRecordId(query.ProductionRecordId.Value), cancellationToken);
            if (record is null) return Result<YieldIndicatorsReport>.Failure(QualityErrors.ProductionRecordNotFound);
            records = StartedWithin(record, query.From, query.To) ? [record] : [];
        }
        else
        {
            records = await externalProductionService.FetchProductionRecords(null, query.From, query.To,
                cancellationToken);
        }

        var recordIds = records.Select(record => record.Id).ToList();
        var assessments = await assessmentRepository.ListByProductionRecordsAsync(recordIds, cancellationToken);
        var wasteRecords = await wasteRepository.ListByProductionRecordsAsync(recordIds, cancellationToken);

        var indicators = records.Select(record => new YieldIndicator(
            record.Id,
            assessments.Where(assessment => assessment.ProductionRecordId == record.Id)
                .Select(assessment => assessment.MeasurementValue(QualityIndicator.YieldPercentage))
                .FirstOrDefault(value => value.HasValue),
            wasteRecords.Where(waste => waste.ProductionRecordId == record.Id)
                .Select(waste => waste.Percentage?.Value).ToList())).ToList();
        return Result<YieldIndicatorsReport>.Success(new YieldIndicatorsReport(query.From, query.To, indicators));
    }

    public async Task<Result<RiceCompositionReport>> Handle(GetRiceCompositionQuery query,
        CancellationToken cancellationToken)
    {
        if (!await externalProductionService.ExistsProductionBatch(query.BatchId, cancellationToken))
            return Result<RiceCompositionReport>.Failure(QualityErrors.ProductionBatchNotFound);

        var records = await externalProductionService.FetchProductionRecords(query.BatchId, null, null,
            cancellationToken);
        var recordIds = records.Select(record => record.Id).ToList();
        var assessments = await assessmentRepository.ListByProductionRecordsAsync(recordIds, cancellationToken);

        var compositions = assessments
            .Select(assessment => new
            {
                assessment,
                whole = assessment.MeasurementValue(QualityIndicator.WholeGrainPercentage),
                broken = assessment.MeasurementValue(QualityIndicator.BrokenGrainPercentage)
            })
            .Where(item => item.whole.HasValue && item.broken.HasValue)
            .Select(item => new RiceComposition(item.assessment.ProductionRecordId, item.whole!.Value,
                item.broken!.Value, item.assessment.AssessedAt))
            .ToList();
        return Result<RiceCompositionReport>.Success(new RiceCompositionReport(query.BatchId, compositions));
    }

    private static ProductionRecordId? ToRecordId(long? value) =>
        value.HasValue ? new ProductionRecordId(value.Value) : null;

    private static bool StartedWithin(ProductionRecordReference record, DateTime? from, DateTime? to)
    {
        var fromUtc = from.HasValue ? Guard.UtcInstant(from.Value, SharedErrors.InvalidDateRange) : (DateTime?)null;
        var toUtc = to.HasValue ? Guard.UtcInstant(to.Value, SharedErrors.InvalidDateRange) : (DateTime?)null;
        return (!fromUtc.HasValue || record.StartedAt >= fromUtc) && (!toUtc.HasValue || record.StartedAt <= toUtc);
    }
}
