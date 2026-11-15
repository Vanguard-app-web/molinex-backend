using Cortex.Mediator.Commands;
using Molinex.Platform.QualityAndYieldControl.Application.CommandServices;
using Molinex.Platform.QualityAndYieldControl.Application.OutboundServices.Acl;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Commands;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Errors;
using Molinex.Platform.QualityAndYieldControl.Domain.Repositories;

namespace Molinex.Platform.QualityAndYieldControl.Application.Internal.CommandServices;

public class QualityCommandService(
    IQualityAssessmentRepository assessmentRepository,
    IWasteRecordRepository wasteRepository,
    IExternalProductionService externalProductionService)
    : IQualityCommandService,
        ICommandHandler<RecordQualityAssessmentCommand, Result<QualityAssessment>>,
        ICommandHandler<RecordWasteCommand, Result<WasteRecord>>
{
    public async Task<Result<QualityAssessment>> Handle(RecordQualityAssessmentCommand command,
        CancellationToken cancellationToken)
    {
        QualityAssessment assessment;
        try
        {
            assessment = new QualityAssessment(command);
        }
        catch (DomainRuleException exception)
        {
            return Result<QualityAssessment>.Failure(exception.Error);
        }

        if (!await externalProductionService.ExistsProductionRecord(assessment.ProductionRecordId, cancellationToken))
            return Result<QualityAssessment>.Failure(QualityErrors.ProductionRecordNotFound);

        await assessmentRepository.AddAsync(assessment, cancellationToken);
        return Result<QualityAssessment>.Success(assessment);
    }

    public async Task<Result<WasteRecord>> Handle(RecordWasteCommand command, CancellationToken cancellationToken)
    {
        WasteRecord wasteRecord;
        try
        {
            wasteRecord = new WasteRecord(command);
        }
        catch (DomainRuleException exception)
        {
            return Result<WasteRecord>.Failure(exception.Error);
        }

        if (!await externalProductionService.ExistsProductionRecord(wasteRecord.ProductionRecordId, cancellationToken))
            return Result<WasteRecord>.Failure(QualityErrors.ProductionRecordNotFound);

        await wasteRepository.AddAsync(wasteRecord, cancellationToken);
        return Result<WasteRecord>.Success(wasteRecord);
    }
}
