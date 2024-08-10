using Domain.Entities;
using TeamTasks.Domain.Common.Core.Primitives.Maybe;

namespace Database.MetricsAndRabbitMessages.Data.Interfaces;

/// <summary>
/// Represents the metrics repository interface.
/// </summary>
public interface IMetricsRepository
{
    /// <summary>
    /// Insert in database the metric entity.
    /// </summary>
    /// <param name="metricEntity">The metric entity.</param>
    /// <returns>Returns <see cref="MetricEntity"/>.</returns>
    System.Threading.Tasks.Task InsertAsync(MetricEntity metricEntity);

    /// <summary>
    /// Get metrics entity by time.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <param name="metricsName">The metric name.</param>
    /// <returns>List by <see cref="MetricEntity"/> classes.</returns>
    Task<Maybe<List<MetricEntity>>> GetByTime(int time, string? metricsName = default);
    
    /// <summary>
    /// Insert any metrics entities in database.
    /// </summary>
    /// <param name="metrics">The enumerable of metrics classes.</param>
    /// <returns>Returns Task info.</returns>
    System.Threading.Tasks.Task InsertRangeAsync(IEnumerable<MetricEntity> metrics);
}