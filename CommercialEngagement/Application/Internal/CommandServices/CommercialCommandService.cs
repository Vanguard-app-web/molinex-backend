using Cortex.Mediator.Commands;
using Molinex.Platform.CommercialEngagement.Application.CommandServices;
using Molinex.Platform.CommercialEngagement.Domain.Model.Aggregates;
using Molinex.Platform.CommercialEngagement.Domain.Model.Commands;
using Molinex.Platform.CommercialEngagement.Domain.Repositories;

namespace Molinex.Platform.CommercialEngagement.Application.Internal.CommandServices;

public class CommercialCommandService(ICommercialInquiryRepository inquiryRepository, TimeProvider timeProvider)
    : ICommercialCommandService, ICommandHandler<SubmitCommercialInquiryCommand, Result<CommercialInquiry>>
{
    public async Task<Result<CommercialInquiry>> Handle(SubmitCommercialInquiryCommand command,
        CancellationToken cancellationToken)
    {
        CommercialInquiry inquiry;
        try
        {
            inquiry = new CommercialInquiry(command, timeProvider.GetUtcNow().UtcDateTime);
        }
        catch (DomainRuleException exception)
        {
            return Result<CommercialInquiry>.Failure(exception.Error);
        }

        await inquiryRepository.AddAsync(inquiry, cancellationToken);
        return Result<CommercialInquiry>.Success(inquiry);
    }
}
