using Microsoft.Extensions.DependencyInjection;
using Molinex.Platform.AssetAndMaintenanceManagement.Application.CommandServices;
using Molinex.Platform.AssetAndMaintenanceManagement.Application.Internal.CommandServices;
using Molinex.Platform.AssetAndMaintenanceManagement.Application.Internal.QueryServices;
using Molinex.Platform.AssetAndMaintenanceManagement.Application.QueryServices;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Repositories;
using Molinex.Platform.AssetAndMaintenanceManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Infrastructure.Configuration;

/// <summary>Registers this bounded context; the host owns persistence, authentication and mediator pipelines.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAssetAndMaintenanceManagement(this IServiceCollection services)
    {
        services.AddScoped<IMachineRepository, MachineRepository>();
        services.AddScoped<IMaintenanceRecordRepository, MaintenanceRecordRepository>();
        services.AddScoped<IMaintenanceCommandService, MaintenanceCommandService>();
        services.AddScoped<IMaintenanceQueryService, MaintenanceQueryService>();
        return services;
    }
}
