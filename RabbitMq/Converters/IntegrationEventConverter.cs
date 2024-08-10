using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TeamTasks.Application.Core.Abstractions.Messaging;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace RabbitMq.Converters;

/// <summary>
/// Represents the integration event JSON converter class.
/// </summary>
public class IntegrationEventConverter 
    : JsonConverter<IIntegrationEvent>
{
    /// <inheritdoc />
    public override IIntegrationEvent? ReadJson(
        JsonReader reader,
        Type objectType,
        IIntegrationEvent? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        if (jsonObject.ToString() != "{}")
        {
            var eventType = jsonObject.GetValue("Type");
            
            if (eventType is not null)
            {
                IIntegrationEvent? integrationEvent = eventType!.Value<string>() switch
                {
                    "TeamTasks.Identity.IntegrationEvents.User.Events.PasswordChanged.UserPasswordChangedIntegrationEvent" => 
                        jsonObject.ToObject<UserPasswordChangedIntegrationEvent>(),
                    "TeamTasks.Identity.IntegrationEvents.User.Events.UserCreated.UserCreatedIntegrationEvent" =>
                        jsonObject.ToObject<UserCreatedIntegrationEvent>(),
                    _ => throw new NotSupportedException($"Unsupported integration event type: {eventType}")
                };

                return integrationEvent;
            }
        }

        return default;
    }

    /// <inheritdoc />
    public override void WriteJson(
        JsonWriter writer,
        IIntegrationEvent? value,
        JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}