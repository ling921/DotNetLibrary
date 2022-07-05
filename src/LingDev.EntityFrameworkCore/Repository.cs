using LingDev.EntityFrameworkCore.Entities;
using LingDev.EntityFrameworkCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LingDev.EntityFrameworkCore;

/// <summary>
/// Implementation to base repository methods.
/// </summary>
/// <typeparam name="TDbContext">Type of database context.</typeparam>
/// <typeparam name="TEntity">Type of entity.</typeparam>
public class Repository<TDbContext, TEntity> : IRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class, IEntity
{
    private readonly Lazy<DbSet<TEntity>> _lazyEntities;

    #region Private Fields

    /// <summary>
    /// The database context.
    /// </summary>
    protected readonly TDbContext Context;

    /// <summary>
    /// The <see cref="IUnitOfWork{TDbContext}"/> to handle transaction.
    /// </summary>
    protected readonly IUnitOfWork<TDbContext> UnitOfWork;

    /// <summary>
    /// The entities of this repository.
    /// </summary>
    protected virtual DbSet<TEntity> Entities => _lazyEntities.Value;

    #endregion Private Fields

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TDbContext, TEntity}"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    protected Repository(
        TDbContext context,
        IUnitOfWork<TDbContext> unitOfWork)
    {
        Context = context;
        UnitOfWork = unitOfWork;
        _lazyEntities = new Lazy<DbSet<TEntity>>(() => Context.Set<TEntity>());
    }

    #endregion Constructor

    #region Create

    /// <inheritdoc/>
    public virtual async Task<int> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return UnitOfWork.IsInTransaction
            ? -1
            : await Context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<int> CreateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await Context.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        return UnitOfWork.IsInTransaction
            ? -1
            : await Context.SaveChangesAsync(cancellationToken);
    }

    #endregion Create

    #region Read

    /// <inheritdoc/>
    public virtual IQueryable<TEntity> Query(QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll)
        => trackingBehavior switch
        {
            QueryTrackingBehavior.TrackAll => Entities.AsTracking(),
            QueryTrackingBehavior.NoTracking => Entities.AsNoTracking(),
            QueryTrackingBehavior.NoTrackingWithIdentityResolution => Entities.AsNoTrackingWithIdentityResolution(),
            _ => Entities
        };

    /// <inheritdoc/>
    public virtual Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return Entities.ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Entities.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Entities.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Entities.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Entities.LastOrDefaultAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Entities.AnyAsync(predicate, cancellationToken);
    }

    #endregion Read

    #region Update

    /// <inheritdoc/>
    public virtual async Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        Context.Update(entity);
        return UnitOfWork.IsInTransaction
            ? -1
            : await Context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<int> UpdateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        Context.UpdateRange(entities);
        return UnitOfWork.IsInTransaction
            ? -1
            : await Context.SaveChangesAsync(cancellationToken);
    }

    #endregion Update

    #region Delete

    /// <inheritdoc/>
    public virtual async Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Remove(entity);
        return UnitOfWork.IsInTransaction
            ? -1
            : await Context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        Context.RemoveRange(Entities.Where(predicate));
        return UnitOfWork.IsInTransaction
            ? -1
            : await Context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<int> DeleteManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        Context.RemoveRange(entities);
        return UnitOfWork.IsInTransaction
            ? -1
            : await Context.SaveChangesAsync(cancellationToken);
    }

    #endregion Delete
}

/// <summary>
/// Implementation to base repository methods.
/// </summary>
/// <typeparam name="TDbContext">Type of database context.</typeparam>
/// <typeparam name="TEntity">Type of entity.</typeparam>
/// <typeparam name="TKey">Type of entity primary key.</typeparam>
public class Repository<TDbContext, TEntity, TKey> : Repository<TDbContext, TEntity>, IRepository<TEntity, TKey>, IRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TDbContext, TEntity, TKey}"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="unitOfWork">The work unit.</param>
    protected Repository(
        TDbContext context,
        IUnitOfWork<TDbContext> unitOfWork)
        : base(context, unitOfWork)
    {
    }

    #region Read

    /// <inheritdoc/>
    public Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return Entities.SingleOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    #endregion Read

    #region Update

    /// <inheritdoc/>
    public async Task<int> UpdateAsync(TKey id, IDictionary<string, object?> properties, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            return 0;
        }

        var entityEntry = Context.Entry(entity);
        foreach (var propertyEntry in entityEntry.Properties)
        {
            if (propertyEntry.Metadata.Name == "Id" || !properties.ContainsKey(propertyEntry.Metadata.Name))
                continue;

            var newValue = properties[propertyEntry.Metadata.Name];
            if (newValue == null)
            {
                if (propertyEntry.Metadata.ClrType.IsClass)
                {
                    propertyEntry.CurrentValue = propertyEntry.Metadata.ClrType == typeof(string)
                        ? string.Empty
                        : null;
                }
            }
            else
            {
                if (propertyEntry.Metadata.ClrType.IsAssignableFrom(newValue.GetType()))
                {
                    propertyEntry.CurrentValue = newValue;
                }
            }
        }

        return UnitOfWork.IsInTransaction
             ? -1
             : await Context.SaveChangesAsync(cancellationToken);
    }

    #endregion Update

    #region Delete

    /// <inheritdoc/>
    public async Task<int> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            return 0;
        }

        Context.Entry(entity).State = EntityState.Deleted;

        return UnitOfWork.IsInTransaction
             ? -1
             : await Context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> DeleteManyAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        await foreach (var entity in Entities.Where(e => ids.Contains(e.Id)).AsAsyncEnumerable())
        {
            Context.Entry(entity).State = EntityState.Deleted;
        }

        return UnitOfWork.IsInTransaction
             ? -1
             : await Context.SaveChangesAsync(cancellationToken);
    }

    #endregion Delete
}
