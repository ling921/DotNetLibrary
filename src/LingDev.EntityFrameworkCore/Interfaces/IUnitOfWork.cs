using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace LingDev.EntityFrameworkCore.Interfaces;

/// <summary>
/// The unit of work for database transaction.
/// </summary>
/// <typeparam name="TDbContext">Type of databse context.</typeparam>
public interface IUnitOfWork<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Whether the transaction is in progress.
    /// </summary>
    bool IsInTransaction { get; }

    /// <summary>
    /// <inheritdoc cref="DatabaseFacade.BeginTransactionAsync(CancellationToken)"/>
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <returns><inheritdoc cref="DatabaseFacade.BeginTransactionAsync(CancellationToken)"/></returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="IDbContextTransaction.CreateSavepointAsync(string, CancellationToken)"/>
    /// </summary>
    /// <param name="name">
    /// <inheritdoc cref="IDbContextTransaction.CreateSavepointAsync(string, CancellationToken)"/>
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <returns><inheritdoc cref="IDbContextTransaction.CreateSavepointAsync(string, CancellationToken)"/></returns>
    Task CreateSavePointAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits all changes made to the database in the current transaction asynchronously. Rolling
    /// back if commit failed.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <returns><inheritdoc cref="IDbContextTransaction.CommitAsync(CancellationToken)"/></returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits all changes made to the database in the current transaction asynchronously. Rolling
    /// back to save point if commit failed.
    /// </summary>
    /// <param name="savePointName">Name of save point.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <returns><inheritdoc cref="IDbContextTransaction.CommitAsync(CancellationToken)"/></returns>
    Task<int> CommitAsync(string savePointName, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="IDbContextTransaction.RollbackAsync(CancellationToken)"/>
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <returns><inheritdoc cref="IDbContextTransaction.RollbackAsync(CancellationToken)"/></returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="IDbContextTransaction.RollbackToSavepointAsync(string, CancellationToken)"/>
    /// </summary>
    /// <param name="name">
    /// <inheritdoc cref="IDbContextTransaction.RollbackToSavepointAsync(string, CancellationToken)"/>
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <returns><inheritdoc cref="IDbContextTransaction.RollbackToSavepointAsync(string, CancellationToken)"/></returns>
    Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default);
}
