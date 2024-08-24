using Domain.Core.Utility;

namespace AiChat.API.Common.DependencyInjection;

internal static class DiCaching
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "Identity_";
        });
        
        return services;
    }
}