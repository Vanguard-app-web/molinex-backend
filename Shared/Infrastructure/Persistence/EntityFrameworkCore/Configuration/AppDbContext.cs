using Microsoft.EntityFrameworkCore;
using Molinex.Platform.AssetAndMaintenanceManagement.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Molinex.Platform.CommercialEngagement.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Molinex.Platform.ProductionManagement.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Molinex.Platform.QualityAndYieldControl.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;

namespace Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>Single database context of the modular monolith. Each bounded context owns its mappings.</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplySharedConfiguration();
        builder.ApplyProductionManagementConfiguration();
        builder.ApplyQualityAndYieldControlConfiguration();
        builder.ApplyAssetAndMaintenanceConfiguration();
        builder.ApplyIdentityAndAccessConfiguration();
        builder.ApplyCommercialEngagementConfiguration();
        builder.UseSnakeCaseNamingConvention();
    }
}
