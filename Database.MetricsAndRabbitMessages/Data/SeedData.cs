using Domain.Entities;
using TeamTasks.Database.MetricsAndRabbitMessages;
using TeamTasks.Domain.Entities;

namespace Database.MetricsAndRabbitMessages.Data;

/// <summary>
/// Represents the seed data class.
/// </summary>
internal static class SeedData
{
    /// <summary>
    /// Seeding the data.
    /// </summary>
    /// <param name="context">The common mongo database context.</param>
    public static void SeedingData(ICommonMongoDbContext context)
    {
         context.Metrics.InsertOne(
            new MetricEntity(
                "TeamTasks_requests_total", 
                "Total number of requests."));
        
        context.RabbitMessages.InsertOne(
            new RabbitMessage{Description = "skfskdlfkjdsnfkk"});
    }
}