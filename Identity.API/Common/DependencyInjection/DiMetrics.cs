using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Domain.Core.Utility;

namespace Identity.Api.Common.DependencyInjection;

internal static class DiMetrics
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="loggingBuilder">The logging builder.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddMetricsOpenTelemetry(
        this IServiceCollection services,
        ILoggingBuilder loggingBuilder)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));

        services.AddOpenTelemetry();
        
        TracerProviderBuilder tracerProviderBuilder = new TracerProviderBuilderBase();
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddSqlClientInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddOtlpExporter()
            .Build();

        MeterProviderBuilder meterProviderBuilder = new MeterProviderBuilderBase();
        meterProviderBuilder.AddPrometheusExporter();
             
        loggingBuilder.AddOpenTelemetry(logging =>
            logging.AddOtlpExporter());
        
        return services;
    }
}