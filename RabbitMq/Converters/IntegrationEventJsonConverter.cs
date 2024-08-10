using System.Text.Json;
using System.Text.Json.Serialization;
using TeamTasks.Application.Core.Abstractions.Messaging;

namespace RabbitMq.Converters;

/// <summary>
/// Represents the integration event System.Text.Json.JsonSerializer class.
/// </summary>
public sealed class IntegrationEventJsonConverter: JsonConverter<IIntegrationEvent>
{
    /// <inheritdoc />
    public override IIntegrationEvent? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer, 
        IIntegrationEvent value, 
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Type", value.GetType().ToString());
        IIntegrationEvent concreteIntegrationEvent = value.GetType().Name switch
        {
            nameof(UserCreatedIntegrationEvent) =>
                (UserCreatedIntegrationEvent)value,
            nameof(UserPasswordChangedIntegrationEvent) =>
                (UserPasswordChangedIntegrationEvent)value,
            _ => value
        };
        writer.WriteString("UserId", concreteIntegrationEvent.Id);
        writer.WriteEndObject();
    }
}