using System.Reflection;
using Application.ApiHelpers.Configurations;
using Domain.Core.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceDefaults;

public static class SwaggerExtensions
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="majorVersion">The major version.</param>
    /// <param name="minorVersion">The minor version.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddSwagger(
        this IServiceCollection services, 
        Assembly assembly,
        int majorVersion,
        int minorVersion)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));
        
        services.AddSwachbackleService(
            assembly,
            assembly.GetName().Name!);
        
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(majorVersion, minorVersion);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
        
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        
        return services;
    }
}