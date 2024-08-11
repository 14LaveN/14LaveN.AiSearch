using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Application.ApiHelpers.Configurations;
using Domain.Core.Utility;

namespace Identity.Api.Common.DependencyInjection;

internal static class DiSwagger
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddSwagger(
        this IServiceCollection services)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));
        
        services.AddSwachbackleService(
            Assembly.GetExecutingAssembly(),
            Assembly.GetExecutingAssembly().GetName().Name!);
        
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
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