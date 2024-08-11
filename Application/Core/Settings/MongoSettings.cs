using System.ComponentModel.DataAnnotations;

namespace Application.Core.Settings;

/// <summary>
/// Represents the mongo settings class.
/// </summary>
public sealed class MongoSettings
{
    /// <summary>
    /// Gets mongo settings key.
    /// </summary>
    public static string MongoSettingsKey = "MongoConnection";
    
    /// <summary>
    /// Gets or sets connection string.
    /// </summary>
    [Required, Url]
    public string ConnectionString { get; init; } = null!;

    /// <summary>
    /// Gets or sets database name.
    /// </summary>
    [Required]
    public string Database { get; init; } = null!;

    /// <summary>
    /// Gets or sets Rabbit Messages Collection Name.
    /// </summary>
    [Required]
    public string RabbitMessagesCollectionName { get; init; } = null!;

    /// <summary>
    /// Gets or sets Metrics Collection Name.
    /// </summary>
    [Required]
    public string MetricsCollectionName { get; init; } = null!;
}