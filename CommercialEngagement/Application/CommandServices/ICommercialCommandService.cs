using Molinex.Platform.CommercialEngagement.Domain.Model.Aggregates;
using Molinex.Platform.CommercialEngagement.Domain.Model.Commands;

namespace Molinex.Platform.CommercialEngagement.Application.CommandServices;

/// <summary>Application contract for Commercial Engagement commands, dispatched through the mediator.</summary>
public interface ICommercialCommandService
{
    Task<Result<CommercialInquiry>> Handle(SubmitCommercialInquiryCommand command,
        CancellationToken cancellationToken);
}
