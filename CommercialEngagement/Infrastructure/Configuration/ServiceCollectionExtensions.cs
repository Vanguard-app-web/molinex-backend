using Microsoft.Extensions.DependencyInjection;
using Molinex.Platform.CommercialEngagement.Application.CommandServices;
using Molinex.Platform.CommercialEngagement.Application.Internal.CommandServices;
using Molinex.Platform.CommercialEngagement.Application.Internal.QueryServices;
using Molinex.Platform.CommercialEngagement.Application.QueryServices;
using Molinex.Platform.CommercialEngagement.Domain.Repositories;
using Molinex.Platform.CommercialEngagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Molinex.Platform.CommercialEngagement.Infrastructure.Configuration;

/// <summary>Registers Commercial Engagement; the host owns the database, mediator and authentication.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommercialEngagement(this IServiceCollection services)
    {
        services.AddScoped<ICommercialInquiryRepository, CommercialInquiryRepository>();
        services.AddScoped<ICommercialCommandService, CommercialCommandService>();
        services.AddScoped<ICommercialQueryService, CommercialQueryService>();
        return services;
    }
}
