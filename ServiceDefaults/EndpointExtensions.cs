using System.Reflection;
using Application.Core.Abstractions.Endpoint;
using Domain.Core.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ServiceDefaults;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(
        this IHostApplicationBuilder builder,
        Assembly assembly)
    {
        Ensure.NotNull(builder, "Builder is required", nameof(builder));

        IServiceCollection services = builder.Services;
        
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