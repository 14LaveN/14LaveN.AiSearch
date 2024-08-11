using System.ComponentModel.DataAnnotations;

namespace RabbitMq.Abstractions.Settings;

/// <summary>
/// Represents the message broker settings.
/// </summary>
public sealed class MessageBrokerSettings
{
    public const string SettingsKey = "MessageBroker";

    /// <summary>
    /// Gets or sets the host name.
    /// </summary>
    [Required, Url] public static readonly string AmqpLink =
        "amqps://dgpswpjt:tbQvnOh93n-sdqDMjXAjfB53OiShmOka@chimpanzee.rmq.cloudamqp.com/dgpswpjt";

    /// <summary>
    /// Gets or sets the queue name.
    /// </summary>
    [Required]
    public string? QueueName { get; set; }
}