using Molinex.Platform.CommercialEngagement.Application.QueryServices;
using Molinex.Platform.CommercialEngagement.Domain.Model.Aggregates;
using Molinex.Platform.CommercialEngagement.Domain.Model.Queries;
using Molinex.Platform.CommercialEngagement.Domain.Repositories;

namespace Molinex.Platform.CommercialEngagement.Application.Internal.QueryServices;

public class CommercialQueryService(ICommercialInquiryRepository inquiryRepository) : ICommercialQueryService
{
    public async Task<CommercialInquiry?> Handle(GetCommercialInquiryByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await inquiryRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }
}
