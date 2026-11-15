using Molinex.Platform.CommercialEngagement.Domain.Model.Commands;
using Molinex.Platform.CommercialEngagement.Domain.Model.Errors;
using Molinex.Platform.CommercialEngagement.Domain.Model.Events;
using Molinex.Platform.CommercialEngagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.CommercialEngagement.Domain.Model.Aggregates;

public class CommercialInquiry : AuditableAggregateRoot
{
    /// <summary>Text limit kept from the base backend; the landing page does not define a shorter one.</summary>
    public const int MaxTextLength = 2048;

    private CommercialInquiry()
    {
        FullName = string.Empty;
        CompanyName = string.Empty;
        Email = null!;
        Message = string.Empty;
    }

    public CommercialInquiry(SubmitCommercialInquiryCommand command, DateTime utcNow)
    {
        FullName = Guard.RequiredText(command.FullName, MaxTextLength, CommercialErrors.InvalidFullName);
        CompanyName = Guard.RequiredText(command.CompanyName, MaxTextLength, CommercialErrors.InvalidCompanyName);
        Email = new ContactEmail(command.Email);
        Message = Guard.RequiredText(command.Message, MaxTextLength, CommercialErrors.InvalidMessage);
        SubmittedAt = utcNow;
        AddDomainEvent(() => new CommercialInquirySubmittedEvent(Id, CompanyName));
    }

    public CommercialInquiryId Id { get; private set; }
    public string FullName { get; private set; }
    public string CompanyName { get; private set; }
    public ContactEmail Email { get; private set; }
    public string Message { get; private set; }
    public DateTime SubmittedAt { get; private set; }
}
