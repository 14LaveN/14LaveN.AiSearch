using System.Linq.Expressions;

namespace Domain.Core.Extensions;

/// <summary>
/// Represents the where if query extension static class.
/// </summary>
public static class WhereIfQueryExtension
{
    /// <summary>
    /// Checks and collects data with condition.
    /// </summary>
    /// <param name="source">The source data.</param>
    /// <param name="condition">The boolean condition.</param>
    /// <param name="predicate">The predicate.</param>
    /// <typeparam name="T">The generic type.</typeparam>
    /// <returns>Returns sorted data.</returns>
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        if (condition)
            return source.Where(predicate);
        return source;
    }
}