using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LingDev.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for <see cref="IQueryable{TSource}"/>.
/// </summary>
public static class QueryableExtensions
{
    #region WhereIf

    /// <summary>
    /// If condition filters a sequence of values based on a predicate, otherwise not.
    /// </summary>
    /// <param name="source">An <see cref="IQueryable{TSource}"/> to filter.</param>
    /// <param name="condition">Condition of filter</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <exception cref="ArgumentNullException"/>
    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    #endregion WhereIf

    #region Pagination

    /// <summary>
    /// Asynchronously get paged results.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IQueryable{T}"/> to filter.</param>
    /// <param name="pageNumber">The current active page, defaults to the first page.</param>
    /// <param name="pageSize">The number of items per page, defaults to 10.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The total number of items to be paged, and items of current active page.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static async ValueTask<(int Total, IEnumerable<TSource> Items)> ToPagedAsync<TSource>(
        this IQueryable<TSource> source,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number cannot be less than 1.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size cannot be less than 1.");

        var total = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync(cancellationToken);

        return (total, items);
    }

    /// <summary>
    /// Asynchronously get paged results, and projects each element of a sequence into a new form.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the elements of result.</typeparam>
    /// <param name="source">An <see cref="IQueryable{T}"/> to filter.</param>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <param name="pageNumber">The current active page, defaults to the first page.</param>
    /// <param name="pageSize">The number of items per page, defaults to 10.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The total number of items to be paged, and items of current active page.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static async ValueTask<(int Total, IEnumerable<TResult> Items)> ToPagedAsync<TSource, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TSource, TResult>> selector,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (selector == null)
            throw new ArgumentNullException(nameof(selector));
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number cannot be less than 1.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size cannot be less than 1.");

        var total = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .Select(selector)
                                .ToListAsync(cancellationToken);

        return (total, items);
    }

    /// <summary>
    /// Asynchronously get paged results, and projects each element of a sequence into a new form.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the elements of result.</typeparam>
    /// <param name="source">An <see cref="IQueryable{T}"/> to filter.</param>
    /// <param name="projector">A projector to project each element of a sequence into a new form.</param>
    /// <param name="pageNumber">The current active page, defaults to the first page.</param>
    /// <param name="pageSize">The number of items per page, defaults to 10.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The total number of items to be paged, and items of current active page.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static async ValueTask<(int Total, IEnumerable<TResult> Items)> ToPagedAsync<TSource, TResult>(
        this IQueryable<TSource> source,
        Func<IQueryable<TSource>, IQueryable<TResult>> projector,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (projector == null)
            throw new ArgumentNullException(nameof(projector));
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number cannot be less than 1.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size cannot be less than 1.");

        var total = await source.CountAsync(cancellationToken);
        var sourceItems = source.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize);
        var items = await projector(sourceItems).ToListAsync(cancellationToken);

        return (total, items);
    }

    #endregion Pagination
}
