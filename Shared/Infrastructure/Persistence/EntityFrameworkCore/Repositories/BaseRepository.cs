using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>EF Core implementation of the operations common to every aggregate repository.</summary>
public abstract class BaseRepository<TEntity>(AppDbContext context) : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext Context = context;

    private IProperty KeyProperty =>
        Context.Model.FindEntityType(typeof(TEntity))!.FindPrimaryKey()!.Properties[0];

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public async Task<TEntity?> FindByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().FirstOrDefaultAsync(BuildKeyEqualsPredicate(id), cancellationToken);
    }

    public async Task<TEntity?> FindByIdAsNoTrackingAsync(long id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().AsNoTracking()
            .FirstOrDefaultAsync(BuildKeyEqualsPredicate(id), cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().AsNoTracking().AnyAsync(BuildKeyEqualsPredicate(id), cancellationToken);
    }

    private Expression<Func<TEntity, bool>> BuildKeyEqualsPredicate(long id)
    {
        var keyProperty = KeyProperty;
        var converter = keyProperty.GetTypeMapping().Converter;
        var key = converter is null ? id : converter.ConvertFromProvider(id)!;
        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var property = Expression.Property(parameter, keyProperty.Name);
        var equals = Expression.Equal(property, Expression.Constant(key, property.Type));
        return Expression.Lambda<Func<TEntity, bool>>(equals, parameter);
    }
}
