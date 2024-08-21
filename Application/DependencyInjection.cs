using Microsoft.Extensions.DependencyInjection;
using Application.Core.Abstractions;
using Application.Core.Helpers.Metric;

namespace Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentException();
        
        services
            .AddScoped<CreateMetricsHelper>()
            .AddScoped<SaveChangesResult>();
        
        services
            .AddResponseCaching(options =>
            {
                options.UseCaseSensitivePaths = false; 
                options.MaximumBodySize = 1024; 
            })
            .AddMemoryCache(options =>
            {
                options.TrackLinkedCacheEntries = true;
                options.TrackStatistics = true;
            })
            .AddDistributedMemoryCache(options =>
            {
                options.TrackStatistics = true;
                options.TrackLinkedCacheEntries = true;
            });
        
        return services;
    }
}