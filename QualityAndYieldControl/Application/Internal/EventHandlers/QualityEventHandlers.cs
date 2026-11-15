using Molinex.Platform.QualityAndYieldControl.Domain.Model.Events;
using Molinex.Platform.Shared.Application.Internal.EventHandlers;

namespace Molinex.Platform.QualityAndYieldControl.Application.Internal.EventHandlers;

public class QualityEventHandlers(ILogger<QualityEventHandlers> logger)
    : IEventHandler<QualityAssessmentRecordedEvent>, IEventHandler<WasteRecordedEvent>
{
    public Task Handle(QualityAssessmentRecordedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Quality assessment {AssessmentId} recorded for production record {ProductionRecordId}",
            domainEvent.AssessmentId.Value, domainEvent.ProductionRecordId.Value);
        return Task.CompletedTask;
    }

    public Task Handle(WasteRecordedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Waste record {WasteRecordId} recorded for production record {ProductionRecordId}",
            domainEvent.WasteRecordId.Value, domainEvent.ProductionRecordId.Value);
        return Task.CompletedTask;
    }
}
