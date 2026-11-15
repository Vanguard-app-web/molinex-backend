using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>Single place where pending changes are written; invoked by the transaction pipeline behavior.</summary>
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
