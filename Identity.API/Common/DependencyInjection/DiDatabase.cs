using Domain.Core.Utility;
using Persistence;
using Database.MetricsAndRabbitMessages;
using Identity.API.Persistence;

namespace Identity.Api.Common.DependencyInjection;

internal static class DiDatabase
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environment">The environemnt.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));

        services
            .AddUserDatabase(configuration)
            .AddMongoDatabase(configuration);
        
        string pathToFirebaseConfig = environment.IsDevelopment() 
            ? @"G:\DotNetProjects\TeamTasks\firebase.json" 
            : "/app/firebase.json";

        //TODO FirebaseApp.Create(new AppOptions
        //TODO {
        //TODO     Credential = GoogleCredential.FromFile(pathToFirebaseConfig),
        //TODO });
        
        return services;
    }
}