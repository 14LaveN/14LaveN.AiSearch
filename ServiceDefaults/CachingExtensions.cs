using Application.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServiceDefaults;

public static class CachingExtensions
{
    public static IServiceCollection AddCachingDefaults(this IHostApplicationBuilder builder, string instanceName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        IConfiguration configuration = builder.Configuration;
        IServiceCollection services = builder.Services;

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
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionStringOrThrow("Redis");
            options.InstanceName = instanceName;
        });
        
        return services;
    }
}