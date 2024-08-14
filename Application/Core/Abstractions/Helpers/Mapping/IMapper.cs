namespace Application.Core.Abstractions.Helpers.Mapping;

/// <summary>
/// Represents the mapper interface.
/// </summary>
public interface IMapper<in TSource, out TDestination>
{
    /// <summary>
    /// Map some source entity to a destination.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>Returns the destination object.</returns>
    public TDestination Map(TSource source);
}