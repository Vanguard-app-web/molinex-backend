using Molinex.Platform.CommercialEngagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.CommercialEngagement.Domain.Model.Events;

public record CommercialInquirySubmittedEvent(CommercialInquiryId InquiryId, string CompanyName) : IEvent;
