using Molinex.Platform.CommercialEngagement.Domain.Model.Aggregates;
using Molinex.Platform.CommercialEngagement.Domain.Repositories;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Molinex.Platform.CommercialEngagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class CommercialInquiryRepository(AppDbContext context)
    : BaseRepository<CommercialInquiry>(context), ICommercialInquiryRepository;
