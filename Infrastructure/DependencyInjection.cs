using Infrastructure.Authentication;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Infrastructure.Common;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Helpers.JWT;

namespace Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentException();

        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Program>();
            
            x.NotificationPublisher = new TaskWhenAllPublisher();
            x.NotificationPublisherType = typeof(TaskWhenAllPublisher);
        });
        
        services.AddScoped<IDateTime, MachineDateTime>();
        services.AddScoped<IUserIdentifierProvider, UserIdentifierProvider>();
        services.AddScoped<IPermissionProvider, PermissionProvider>();
        
        return services;
    }
}