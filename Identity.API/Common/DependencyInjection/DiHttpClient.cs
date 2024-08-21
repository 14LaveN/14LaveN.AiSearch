using System.Net;
using System.Threading.RateLimiting;
using Application.ApiHelpers.Middlewares.DelegatingHandlers;
using Domain.Core.Utility;
using Identity.API.Common.Refit.Users;
using Microsoft.AspNetCore.ResponseCompression;
using Refit;

namespace Identity.API.Common.DependencyInjection;

public static class DiHttpClient
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="webHost">The web host.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddHttpClientExtensions(
         this IServiceCollection services,
         IWebHostBuilder webHost)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));

        webHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 5000, listenOptions =>
            {
                listenOptions.UseConnectionLogging();
            });
    
            options.Limits.MaxRequestBodySize = 10 * 1024; // 10 KB
            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
            options.Limits.MaxConcurrentConnections = 100;
            options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
        });

        var refitSettings = new RefitSettings
        {
            ExceptionFactory = async response =>
            {
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return new Exception($"API Error: {content}");
                }
                return null;
            }
        };

        services.AddTransient<LoggingHandler>();
        services
            .AddRefitClient<IUsersClient>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5000"))
            .AddHttpMessageHandler<LoggingHandler>();
        
        return services;
    }
    
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="webHost">The web host.</param>
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