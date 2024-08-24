using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Application.Core.Abstractions.Endpoint;
using Domain.Core.Utility;

namespace Identity.Api.Common.DependencyInjection;

internal static class DiEndpoints
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembly">The assembly.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly)
    {
        Ensure.NotNull(services, "Services is required", nameof(services));
        
        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }
    
    /// <summary>
    /// The map endpoints in some assembly method.
    /// </summary>
    /// <param name="app">The web application builder.</param>
    /// <param name="routeGroupBuilder">The route group builder.</param>
    /// <returns>Returns the modify application builder.</returns>
    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services
            .GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder =
            routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}