using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Commands;

namespace Molinex.Platform.QualityAndYieldControl.Application.CommandServices;

/// <summary>Application contract for Quality and Yield Control commands, dispatched through the mediator.</summary>
public interface IQualityCommandService
{
    Task<Result<QualityAssessment>> Handle(RecordQualityAssessmentCommand command,
        CancellationToken cancellationToken);

    Task<Result<WasteRecord>> Handle(RecordWasteCommand command, CancellationToken cancellationToken);
}
