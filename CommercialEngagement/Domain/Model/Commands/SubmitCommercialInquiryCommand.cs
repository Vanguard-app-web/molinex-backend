using Cortex.Mediator.Commands;
using Molinex.Platform.CommercialEngagement.Domain.Model.Aggregates;

namespace Molinex.Platform.CommercialEngagement.Domain.Model.Commands;

public record SubmitCommercialInquiryCommand(
    string FullName,
    string CompanyName,
    string Email,
    string Message) : ICommand<Result<CommercialInquiry>>;
