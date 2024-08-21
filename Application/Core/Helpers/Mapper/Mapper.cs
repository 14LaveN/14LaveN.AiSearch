using System.Linq.Expressions;
using System.Reflection;
using Application.Core.Abstractions.Helpers.Mapping;

namespace Application.Core.Helpers.Mapper;

/// <summary>
/// Represents the optimized mapper class.
/// </summary>
/// <typeparam name="TSource">The generic source type.</typeparam>
/// <typeparam name="TDestination">The generic destination type.</typeparam>
public sealed class Mapper<TSource, TDestination>
    : IMapper<TSource, TDestination>
    where TDestination : new()
{
    private static readonly Dictionary<string, Action<TSource, TDestination>> PropertyMap = new();

    /// <summary>
    /// Initialize the optimized mapper.
    /// </summary>
    static Mapper()
    {
        var sourceProperties = typeof(TSource)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = typeof(TDestination)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo sourceProperty in sourceProperties)
        {
            PropertyInfo? destinationProperty = destinationProperties
                .FirstOrDefault(destProp =>
                    destProp.Name == sourceProperty.Name
                    && destProp.PropertyType == sourceProperty.PropertyType);

            if (destinationProperty is not null 
                && destinationProperty.CanWrite)
            {
                ParameterExpression sourceParameter = Expression.Parameter(typeof(TSource), "source");
                ParameterExpression destinationParameter = Expression.Parameter(typeof(TDestination), "destination");

                MemberExpression sourceValue = Expression.Property(sourceParameter, sourceProperty);
                MethodCallExpression setDestinationValue = Expression.Call(
                    destinationParameter,
                    destinationProperty.GetSetMethod() ?? throw new InvalidOperationException(), 
                    sourceValue);

                var action = Expression
                    .Lambda<Action<TSource, TDestination>>(
                        setDestinationValue, 
                        sourceParameter, 
                        destinationParameter)
                    .Compile();
                
                PropertyMap.Add(sourceProperty.Name, action);
            }
        }
    }

    ///<inheritdoc />
    public TDestination Map(TSource source)
    {
        var destination = new TDestination();

        if (source is null)
            return destination;

        foreach (Action<TSource, TDestination> action in PropertyMap.Values)
        {
            action(source, destination);
        }

        return destination;
    }
}