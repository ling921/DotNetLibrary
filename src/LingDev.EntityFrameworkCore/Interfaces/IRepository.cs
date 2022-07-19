using LingDev.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LingDev.EntityFrameworkCore.Interfaces;

/// <summary>
/// Interface to base repository methods.
/// </summary>
/// <typeparam name="TEntity">Type of entity.</typeparam>
public interface IRepository<TEntity>
    where TEntity : class, IEntity
{
    #region Create

    /// <summary>
    /// Add an entity to database. If in a transaction, this won't save to the database immediately.
    /// </summary>
    /// <param name="entity">Entity to insert into database.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    Task<int> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add many entities to database. If in a transaction, this won't save to the database immediately.
    /// </summary>
    /// <param name="entities">Entities to insert into database.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    Task<int> CreateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    #endregion Create

    #region Read

    /// <summary>
    /// Get queryable entities.
    /// </summary>
    /// <param name="trackingBehavior">The entity tracking behavior.</param>
    /// <returns>An <see cref="IQueryable{T}"/> entities.</returns>
    IQueryable<TEntity> Query(QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll);

    /// <summary>
    /// Get all entities.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see
    /// cref="List{T}"/> that contains contains entities that satisfy the condition specified by predicate.
    /// </returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list of entities that satisfy the condition.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see
    /// cref="List{T}"/> that contains contains entities that satisfy the condition specified by predicate.
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="EntityFrameworkQueryableExtensions.SingleOrDefaultAsync{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}}, CancellationToken)"/>
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="EntityFrameworkQueryableExtensions.FirstOrDefaultAsync{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}}, CancellationToken)"/>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="EntityFrameworkQueryableExtensions.LastOrDefaultAsync{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}}, CancellationToken)"/>
    Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="EntityFrameworkQueryableExtensions.AnyAsync{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}}, CancellationToken)"/>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    #endregion Read

    #region Update

    /// <summary>
    /// Update an entity in database. If in a transaction, this won't save to the database immediately.
    /// </summary>
    /// <param name="entity">Entity to update into database.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update many entities in database. If in a transaction, this won't save to the database immediately.
    /// </summary>
    /// <param name="entities">Entities to update into database.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    Task<int> UpdateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    #endregion Update

    #region Delete

    /// <summary>
    /// Remove an entity from database. If in a transaction, this won't save to the database immediately.
    /// </summary>
    /// <param name="entity">Entity to remove from database.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove entities from database that satisfy the condition. If in a transaction, this won't
    /// save to the database immediately.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove many entities from database. If in a transaction, this won't save to the database immediately.
    /// </summary>
    /// <param name="entities">Entities to remove from database.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    Task<int> DeleteManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    #endregion Delete
}

/// <summary>
/// Interface to base repository methods.
/// </summary>
/// <typeparam name="TEntity">Type of entity.</typeparam>
/// <typeparam name="TKey">Type of entity primary key.</typeparam>
public interface IRepository<TEntity, TKey> : IRepository<TEntity>
    where TEntity : class, IEntity<TKey>
{
    #region Read

    /// <summary>
    /// Get entity with the specific Id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>Entity with the specific id or null if not found.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    #endregion Read

    #region Update

    /// <summary>
    /// Update an entity with changed properties.
    /// </summary>
    /// <param name="id">Id of entity.</param>
    /// <param name="properties">Properties to update.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    Task<int> UpdateAsync(TKey id, IDictionary<string, object?> properties, CancellationToken cancellationToken = default);

    #endregion Update

    #region Delete

    /// <summary>
    /// Remove an entity from database by Id.
    /// </summary>
    /// <param name="id">Id of entity.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    Task<int> DeleteAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove many entities from database.
    /// </summary>
    /// <param name="ids">Ids of entity.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    Task<int> DeleteManyAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

    #endregion Delete
}
