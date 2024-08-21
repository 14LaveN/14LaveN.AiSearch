using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMq.Abstractions.Settings;

namespace RabbitMq.Abstractions.HealthChecks;

internal sealed class RabbitMqHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(MessageBrokerSettings.AmqpLink)
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            channel.QueueDeclarePassive("healthcheck-queue");

            return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ is available."));
        }
        catch (BrokerUnreachableException ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ is not reachable.", ex));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ is unhealthy.", ex));
        }
    }
}