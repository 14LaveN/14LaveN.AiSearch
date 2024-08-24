using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMq.Abstractions.Settings;

namespace RabbitMq.Abstractions.HealthChecks;

internal sealed class RabbitMqHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(MessageBrokerSettings.AmqpLink)
            };

            using var connection =await factory.CreateConnectionAsync(cancellationToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken);
            
            await channel.QueueDeclarePassiveAsync("healthcheck-queue", cancellationToken);

            return HealthCheckResult.Healthy("RabbitMQ is available.");
        }
        catch (BrokerUnreachableException ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ is not reachable.", ex);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ is unhealthy.", ex);
        }
    }
}