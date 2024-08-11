using Domain.Entities;
using MongoDB.Driver;
using Domain.Entities;

namespace Database.MetricsAndRabbitMessages;

/// <summary>
/// The common mogno database context interface.
/// </summary>
public interface ICommonMongoDbContext
{
    /// <summary>
    /// Gets metrics mongo collection.
    /// </summary>
    IMongoCollection<MetricEntity> Metrics { get; }

    /// <summary>
    /// Gets rabbit messages mongo collection.
    /// </summary>
    IMongoCollection<RabbitMessage> RabbitMessages { get; }
}