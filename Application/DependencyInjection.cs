using Microsoft.Extensions.DependencyInjection;
using TeamTasks.Application.Core.Abstractions;
using TeamTasks.Application.Core.Helpers.Metric;

namespace TeamTasks.Application;

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
        
        services.AddScoped<CreateMetricsHelper>();
        services.AddScoped<SaveChangesResult>();
        
        return services;
    }
}