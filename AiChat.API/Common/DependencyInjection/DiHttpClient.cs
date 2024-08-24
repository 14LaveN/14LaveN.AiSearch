using System.Threading.RateLimiting;
using Domain.Core.Utility;
using Microsoft.AspNetCore.ResponseCompression;

namespace Identity.API.Common.DependencyInjection;

public static class DiHttpClient
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddHttpHelpers(
        this IServiceCollection services)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
            options.AddPolicy("fixed", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.User.Identity?.Name?.ToString(),
                    factory: _ => 
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromSeconds(10),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 2
                        }));
        });

        services
            .AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
            })
            .Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Optimal;
            })
            .Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.SmallestSize;
            });
        return services;
    }
}