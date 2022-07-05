using LingDev.EntityFrameworkCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace LingDev.EntityFrameworkCore;

/// <summary>
/// The unit of work for database transaction.
/// </summary>
/// <typeparam name="TDbContext">Type of databse context.</typeparam>
public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext>
    where TDbContext : DbContext
{
    private readonly TDbContext _context;
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Logger.
    /// </summary>
    protected readonly ILogger Logger;

    /// <inheritdoc/>
    public bool IsInTransaction => _transaction != null;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="context">Database context.</param>
    public UnitOfWork(ILogger<UnitOfWork<TDbContext>> logger, TDbContext context)
    {
        Logger = logger;
        _context = context;
    }

    /// <inheritdoc/>
    public virtual async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (IsInTransaction)
        {
            Logger.LogError("There is another transaction is in progress.");
            throw new InvalidOperationException("There is another transaction is in progress.");
        }
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task CreateSavePointAsync(string name, CancellationToken cancellationToken = default)
    {
        EnsureTransaction();
        await _transaction!.CreateSavepointAsync(name, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var count = 0;
        EnsureTransaction();
        try
        {
            count = await _context.SaveChangesAsync(cancellationToken);
            await _transaction!.CommitAsync(cancellationToken);
            await _transaction!.DisposeAsync();
            _transaction = null;
        }
        catch
        {
            await _transaction!.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();
        }
        return count;
    }

    /// <inheritdoc/>
    public virtual async Task<int> CommitAsync(string savePointName, CancellationToken cancellationToken = default)
    {
        var count = 0;
        EnsureTransaction();
        try
        {
            count = await _context.SaveChangesAsync(cancellationToken);
            await _transaction!.CommitAsync(cancellationToken);
        }
        catch
        {
            await _transaction!.RollbackToSavepointAsync(savePointName, cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();
        }
        return count;
    }

    /// <inheritdoc/>
    public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        EnsureTransaction();
        await _transaction!.RollbackAsync(cancellationToken);
        await _transaction!.DisposeAsync();
    }

    /// <inheritdoc/>
    public virtual async Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default)
    {
        EnsureTransaction();
        await _transaction!.RollbackToSavepointAsync(name, cancellationToken);
    }

    /// <summary>
    /// Ensure created.
    /// </summary>
    /// <exception cref="InvalidOperationException"/>
    private void EnsureTransaction()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Please begin a transaction first.");
        }
    }
}
