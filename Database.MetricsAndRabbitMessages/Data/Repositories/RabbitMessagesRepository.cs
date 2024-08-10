using Domain.Entities;
using TeamTasks.Database.MetricsAndRabbitMessages.Data.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TeamTasks.Application.Core.Settings;
using TeamTasks.Database.MetricsAndRabbitMessages.Data.Interfaces;
using TeamTasks.Domain.Common.Core.Primitives.Maybe;
using TeamTasks.Domain.Entities;

namespace TeamTasks.Database.MetricsAndRabbitMessages.Data.Repositories;

/// <summary>
/// Represents the generic metrics repository class.
/// </summary>
internal sealed class RabbitMessagesRepository
    : IMongoRepository<RabbitMessage>
{
    private readonly ICommonMongoDbContext _commonMongoDbContext;

    /// <summary>
    /// Login new instance of <see cref="RabbitMessagesRepository"/>.
    /// </summary>
    /// <param name="commonMongoDbContext"></param>
    public RabbitMessagesRepository(
        ICommonMongoDbContext commonMongoDbContext)
    {
        _commonMongoDbContext = commonMongoDbContext;
    }

    /// <inheritdoc />
    public async Task<List<RabbitMessage>> GetAllAsync() =>
        await _commonMongoDbContext.RabbitMessages.Find(_ => true).ToListAsync();

    /// <inheritdoc />
    public async Task<Maybe<List<RabbitMessage>>> GetByTime(
        int time,
        string metricName = default)
    {
        var rabbitMessages = await _commonMongoDbContext
            .RabbitMessages
            .FindAsync<RabbitMessage>(x=>
                x.CreatedAt.Day == DateTime.Today.Day 
                && x.CreatedAt.Month == DateTime.Today.Month
                && x.CreatedAt.Year == DateTime.Today.Year).Result
            .ToListAsync();

        return rabbitMessages;
    }

    /// <inheritdoc />
    public async System.Threading.Tasks.Task InsertAsync(RabbitMessage type) =>
        await _commonMongoDbContext.RabbitMessages.InsertOneAsync(type);

    /// <inheritdoc />
    public async System.Threading.Tasks.Task InsertRangeAsync(IEnumerable<RabbitMessage> types) =>
        await _commonMongoDbContext.RabbitMessages.InsertManyAsync(types);

    /// <inheritdoc />
    public async System.Threading.Tasks.Task RemoveAsync(string id) =>
        await _commonMongoDbContext.RabbitMessages.DeleteOneAsync(x => x.Id == id);
}