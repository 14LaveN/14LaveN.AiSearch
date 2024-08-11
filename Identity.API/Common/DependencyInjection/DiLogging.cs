using System.Reflection;
using MongoDB.Bson;
using Domain.Core.Utility;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Application.Core.Extensions;

namespace Identity.Api.Common.DependencyInjection;

internal static class DiLogging
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddLoggingExtension(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(
                configuration.GetConnectionStringOrThrow("ElasticSearch")!))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                IndexFormat = 
                    $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".","-")}-{DateTime.UtcNow:yyyy-MM}",
                NumberOfReplicas = 1,
                NumberOfShards = 2
            })
            .Enrich.FromLogContext()
            .CreateLogger();
        
        return services;
    }
}