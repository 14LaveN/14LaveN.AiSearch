using Application.Core.Extensions;
using Database.MetricsAndRabbitMessages.Data.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundTasks.Tasks;

/// <summary>
/// Represents the save metrics background service.
/// </summary>
internal sealed class SaveMetricsBackgroundService : BackgroundService
{
    private readonly ILogger<SaveMetricsBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMetricsRepository _metricsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveMetricsBackgroundService"/>
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="metricsRepository">The metrics repository.</param>
    public SaveMetricsBackgroundService(
        ILogger<SaveMetricsBackgroundService> logger,
        IServiceProvider serviceProvider,
        IMetricsRepository metricsRepository)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _metricsRepository = metricsRepository;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("SaveMetricsBackgroundService is starting.");

        stoppingToken.Register(() => _logger.LogDebug("SaveMetricsBackgroundService background taskEntity is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug("SaveMetricsBackgroundService background taskEntity is doing background work.");

            await ConsumeSaveMetricsAsync(stoppingToken);

            await System.Threading.Tasks.Task.Delay(10000, stoppingToken);
        }

        _logger.LogDebug("SaveMetricsBackgroundService background taskEntity is stopping.");

        await System.Threading.Tasks.Task.CompletedTask;
    }

    /// <summary>
    /// Consumes the next batch of save metrics.
    /// </summary>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns>The completed taskEntity.</returns>
    private async Task ConsumeSaveMetricsAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation($"Request to save metrics in {nameof(SaveMetricsBackgroundService)}.");

            string counterName = "metrics_counter-key";

            var scope = _serviceProvider.CreateScope();

            var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
            
            Dictionary<string, string> counter = await distributedCache
                .GetRecordAsync<Dictionary<string, string>>(counterName);

            string histogramName = "metrics_request_duration_seconds-key";
            
            string millisecondsInString =
                await distributedCache.GetRecordAsync<string>(histogramName);
            
            if (counter is null )
            {
                _logger.LogWarning($"Counter with same name - {counterName} not found");
            }

            if (millisecondsInString.IsNullOrWhiteSpace())
            {
                _logger.LogWarning($"Histogram with same name - {histogramName} not found");
            }

            if (counter is not null && !millisecondsInString.IsNullOrWhiteSpace())
            {
                var metrics = new List<MetricEntity>
                { 
                    new("TeamTasks_request_duration_seconds",
                        millisecondsInString),
                    new(counter["Name"],
                        counter["Value"])
                };
                
                await _metricsRepository.InsertRangeAsync(metrics);
            }
            
            _logger.LogInformation($"Insert in MongoDb metrics at - {DateTime.UtcNow}");
        }
        catch (Exception exception)
        {
            _logger.LogError($"[SaveMetricsJob]: {exception.Message}");
            throw;
        }
    }
}