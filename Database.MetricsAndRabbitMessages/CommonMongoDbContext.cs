using Database.MetricsAndRabbitMessages.Data;
using Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Application.Core.Settings;
using Database.MetricsAndRabbitMessages.Data;
using Domain.Entities;

namespace Database.MetricsAndRabbitMessages;

/// <summary>
/// The metrics database context.
/// </summary>
public sealed class CommonMongoDbContext 
     : ICommonMongoDbContext
{
    private readonly IMongoDatabase _database = null!;
    private readonly MongoSettings _mongoSettings;

    /// <summary>
    /// Login the new instance <see cref="CommonMongoDbContext"/> class.
    /// </summary>
    /// <param name="settings">The mongo db settings.</param>
    public CommonMongoDbContext(IOptions<MongoSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _mongoSettings = settings.Value;
        
        if (client is not null)
            _database = client.GetDatabase(settings.Value.Database);

        SeedData.SeedingData(this);
    }
    
    /// <inheritdoc/>
    public IMongoCollection<MetricEntity> Metrics =>
        _database.GetCollection<MetricEntity>(_mongoSettings.MetricsCollectionName);

    /// <inheritdoc/>
    public IMongoCollection<RabbitMessage> RabbitMessages =>
        _database.GetCollection<RabbitMessage>(_mongoSettings.RabbitMessagesCollectionName);
}