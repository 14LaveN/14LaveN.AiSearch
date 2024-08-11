using Microsoft.Extensions.DependencyInjection;

namespace RabbitMq.Abstractions;

public interface IEventBusBuilder
{
    public IServiceCollection Services { get; }
}
