using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Database.MetricsAndRabbitMessages.HealthChecks;

internal sealed class MongoDbHealthCheck(ICommonMongoDbContext mongoClient) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var database = mongoClient.Metrics.Database;
            
            await database.RunCommandAsync((Command<BsonDocument>)"{ping:1}", cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy("MongoDB is available.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB is unavailable.", ex);
        }
    }
}