using BackgroundTasks.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BackgroundTasks.Services;
using BackgroundTasks.Services;
using BackgroundTasks.Tasks;

namespace BackgroundTasks.Tasks;

/// <summary>
/// Represents the create report background service.
/// </summary>
internal sealed class CreateReportBackgroundService : BackgroundService
{
    private readonly ILogger<SaveMetricsBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateReportBackgroundService"/>
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public CreateReportBackgroundService(
        ILogger<SaveMetricsBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("CreateReportBackgroundService is starting.");

        stoppingToken.Register(() => _logger.LogDebug("CreateReportBackgroundService background task is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug("CreateReportBackgroundService background task is doing background work.");

            await ProduceReportsAsync(stoppingToken);

            await System.Threading.Tasks.Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
        
        await System.Threading.Tasks.Task.CompletedTask;
    }

    /// <summary>
    /// Produce the next batch of create report.
    /// </summary>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns>The completed task.</returns>
    private async Task ProduceReportsAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();

        var createReportProducer = scope.ServiceProvider.GetRequiredService<ICreateReportProducer>();

        await createReportProducer.ProduceAsync(stoppingToken);
    }
}