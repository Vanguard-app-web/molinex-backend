namespace Molinex.Platform.Shared.Domain.Repositories;

/// <summary>Operations common to every aggregate repository. Specialized queries live in each repository.</summary>
public interface IBaseRepository<TEntity>
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<TEntity?> FindByIdAsNoTrackingAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(long id, CancellationToken cancellationToken = default);
}
