using System.Net;
using Application.ApiHelpers.Middlewares.DelegatingHandlers;
using Domain.Core.Utility;
using Refit;
using Web.Refit.Users;

namespace Web;

public static class DiRefit
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="webHost">The web host.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddRefit(
        this IServiceCollection services,
        IWebHostBuilder webHost)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));

        webHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 6000, listenOptions =>
            {
                listenOptions.UseConnectionLogging();
                
            });
            options.ListenLocalhost(6001, listenOptions =>
            {
                listenOptions.UseHttps();
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
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:5001"))
            .AddHttpMessageHandler<LoggingHandler>();
        
        return services;
    }
}