using Microsoft.EntityFrameworkCore;

namespace Application.Core.Helpers.Pagination;

/// <summary>
/// Represents the generic paged list.
/// </summary>
/// <typeparam name="T">The type of list.</typeparam>
public sealed class PagedList<T>(
    IEnumerable<T> items,
    int page,
    int pageSize,
    int totalCount)
{
    /// <summary>
    /// Gets the current page.
    /// </summary>
    public int Page { get; } = page;

    /// <summary>
    /// Gets the page size. The maximum page size is 100.
    /// </summary>
    public int PageSize { get; } = pageSize;

    /// <summary>
    /// Gets the total number of items.
    /// </summary>
    public int TotalCount { get; } = totalCount;

    /// <summary>
    /// Gets the flag indicating whether the next page exists.
    /// </summary>
    public bool HasNextPage => Page * PageSize < TotalCount;

    /// <summary>
    /// Gets the flag indicating whether the previous page exists.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Gets the items.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; } = items.ToList<T>();

    /// <summary>
    /// Create the paged list with specified query, page number and page size.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns></returns>
    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> query,
        int page,
        int pageSize)
    {
        int totalCount = await query.CountAsync();

        List<T> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new(items, page, pageSize, totalCount);
    }
}