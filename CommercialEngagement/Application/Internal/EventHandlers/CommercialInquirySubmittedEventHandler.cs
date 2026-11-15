using Molinex.Platform.CommercialEngagement.Domain.Model.Events;
using Molinex.Platform.Shared.Application.Internal.EventHandlers;

namespace Molinex.Platform.CommercialEngagement.Application.Internal.EventHandlers;

public class CommercialInquirySubmittedEventHandler(ILogger<CommercialInquirySubmittedEventHandler> logger)
    : IEventHandler<CommercialInquirySubmittedEvent>
{
    public Task Handle(CommercialInquirySubmittedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Commercial inquiry {InquiryId} submitted", domainEvent.InquiryId.Value);
        return Task.CompletedTask;
    }
}
