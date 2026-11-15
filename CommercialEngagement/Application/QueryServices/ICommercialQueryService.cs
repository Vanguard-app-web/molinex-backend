using Molinex.Platform.CommercialEngagement.Domain.Model.Aggregates;
using Molinex.Platform.CommercialEngagement.Domain.Model.Queries;

namespace Molinex.Platform.CommercialEngagement.Application.QueryServices;

public interface ICommercialQueryService
{
    Task<CommercialInquiry?> Handle(GetCommercialInquiryByIdQuery query, CancellationToken cancellationToken);
}
