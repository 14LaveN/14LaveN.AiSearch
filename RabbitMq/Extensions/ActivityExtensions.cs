using System.Diagnostics;

namespace RabbitMq.Extensions;

public static class ActivityExtensions
{
    public static void SetExceptionTags(
        this Activity? activity,
        Exception ex)
    {
        if (activity is null)
        {
            return;
        }

        activity.AddTag("exception.message", ex.Message);
        activity.AddTag("exception.stacktrace", ex.ToString());
        activity.AddTag("exception.type", ex.GetType().FullName);
        activity.SetStatus(ActivityStatusCode.Error);
    }
    
    public static void SetActivityContext(Activity? activity, 
        string routingKey, 
        string operation)
    {
        if (activity is not null)
        {
            // These tags are added demonstrating the semantic conventions of the OpenTelemetry messaging specification
            // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
            activity.SetTag("messaging.system", "rabbitmq");
            activity.SetTag("messaging.destination_kind", "queue");
            activity.SetTag("messaging.operation", operation);
            activity.SetTag("messaging.destination.name", routingKey);
            activity.SetTag("messaging.rabbitmq.routing_key", routingKey);
        }
    }
}