using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Design-time factory used by <c>dotnet ef</c>. Reads the connection string from configuration
///     (environment variables or user secrets); when none is set, a placeholder lets migrations be
///     scaffolded without a database, since scaffolding never opens a connection.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string OfflinePlaceholder = "server=localhost;port=3306;database=molinex_design_time;user=design_time";

    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.Development.json", true)
            .AddUserSecrets<AppDbContextFactory>(true)
            .AddEnvironmentVariables()
            .Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString)) connectionString = OfflinePlaceholder;
        var options = new DbContextOptionsBuilder<AppDbContext>().UseMySQL(connectionString).Options;
        return new AppDbContext(options);
    }
}
