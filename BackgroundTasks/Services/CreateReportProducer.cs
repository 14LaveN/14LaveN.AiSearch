using Database.MetricsAndRabbitMessages.Data.Interfaces;
using Domain.Entities;
using Firebase.Storage;
using Microsoft.Extensions.Logging;
using Quartz.Util;
using BackgroundTasks.Services;
using Database.MetricsAndRabbitMessages.Data.Interfaces;
using IdentityServer4.Extensions;
using Infrastructure.CSV;

namespace BackgroundTasks.Services;

/// <summary>
/// Represents the create report producer class.
/// </summary>
internal sealed class CreateReportProducer(
    IMetricsRepository metricsRepository,
    IMongoRepository<RabbitMessage> rabbitMessagesRepository,
    ILogger<CreateReportProducer> logger)
    : ICreateReportProducer
{
    //TODO Chaange the firebase storage.
    private readonly FirebaseStorage _firebaseStorage = new("todolistmicroservices.appspot.com");
    
    /// <inheritdoc />
    public async Task ProduceAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Request for create the report for metrics and rabbit messages - {DateTime.UtcNow}");
        
        var metricsRequestDuration =
            await metricsRepository.GetByTime(180, "TeamTasks_request_duration_seconds");

        if (metricsRequestDuration.Value.IsNullOrEmpty())
        {
            logger.LogWarning("Metrics request duration with the same time - 180 not found");
        }
        
        var metricsRequestsTotal =
            await metricsRepository.GetByTime(180, "TeamTasks_requests_total");
        
        if (metricsRequestsTotal.Value.IsNullOrEmpty())
        {
            logger.LogWarning("Metrics request total with the same time - 180 not found");
        }

        var rabbitMessages = await rabbitMessagesRepository.GetByTime(180);
        
        if (rabbitMessages.Value.IsNullOrEmpty())
        {
            logger.LogWarning("Rabbit messages with the same time - 180 not found");
        }

        if (!rabbitMessages.Value.IsNullOrEmpty())
        {
            await UploadCsvFile(
                rabbitMessages.Value,
                $"Rabbit statistics for {DateTime.Now.ToLongDateString()}.csv",
                $"Load rabbit messages files in the firebase - {DateTime.UtcNow}",
                stoppingToken);
        }

        if (!metricsRequestsTotal.Value.IsNullOrEmpty()
            || !metricsRequestDuration.Value.IsNullOrEmpty())
        {
            var metrics = metricsRequestsTotal.Value.Union(metricsRequestDuration.Value);
            
            await UploadCsvFile(
                metrics,
                $"Metrics - statistics for {DateTime.Now.ToLongDateString()}.csv",
                $"Load metrics files in the firebase - {DateTime.UtcNow}",
                stoppingToken);
        }
    }

    /// <summary>
    /// Upload the some csv file.
    /// </summary>
    /// <param name="enumerable">The generic enumerable.</param>
    /// <param name="fileName">The file name.
    /// </param>
    /// <param name="loggingMessage">The message after logging.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <typeparam name="T">The generic type.</typeparam>
    private async Task UploadCsvFile<T>(
        IEnumerable<T> enumerable,
        string fileName,
        string loggingMessage,
        CancellationToken stoppingToken)
    {
        CsvBaseService csvService = new CsvBaseService();
        byte[] uploadFile = await csvService.UploadFile(enumerable);
        
        string path =
            $@"G:\DotNetProjects\TeamTasks\Data\{fileName}";
            
        var file = File.Create(path);
            
        await file.WriteAsync(uploadFile, stoppingToken);
        file.Close();
            
        file = File.OpenRead(path);
            
        await _firebaseStorage.Child("uploads").Child(fileName).PutAsync(file);
            
        logger.LogInformation(loggingMessage);
    }
}