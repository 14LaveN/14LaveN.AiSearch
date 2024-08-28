using Infrastructure.Authentication;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Infrastructure.Common;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Helpers.JWT;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        ArgumentNullException.ThrowIfNull(services);

        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Program>();
            
            x.NotificationPublisher = new TaskWhenAllPublisher();
            x.NotificationPublisherType = typeof(TaskWhenAllPublisher);
        });
        
        services
            .AddScoped<IDateTime, MachineDateTime>()
            .AddScoped<IUserIdentifierProvider, UserIdentifierProvider>()
            .AddScoped<IPermissionProvider, PermissionProvider>()
            .TryAddScoped<IUserNameProvider, UserNameProvider>();
        
        return services;
    }
}