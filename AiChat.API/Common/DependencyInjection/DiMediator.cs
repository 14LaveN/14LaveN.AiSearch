using Application.Core.Behaviours;
using Domain.Core.Utility;
using FluentValidation;
using MediatR.NotificationPublishers;

namespace AiChat.API.Common.DependencyInjection;

public static class DiMediator
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddMediatr(this IServiceCollection services)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));

        services.AddValidatorsFromAssemblyContaining<Program>();
        
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Program>();

            //TODO x.RegisterServicesFromAssemblies(typeof(Register.Command).Assembly,
            //TODO         typeof(Register.CommandHandler).Assembly)
            //TODO     .RegisterServicesFromAssemblies(typeof(Login.Command).Assembly,
            //TODO         typeof(Login.CommandHandler).Assembly);
            
            x.AddOpenBehavior(typeof(QueryCachingBehavior<,>))
                //TODO .AddOpenBehavior(typeof(IdentityIdempotentCommandPipelineBehavior<,>))
                .AddOpenBehavior(typeof(BaseTransactionBehavior<,>))
                .AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>))
                .AddOpenBehavior(typeof(ValidationBehaviour<,>))
                .AddOpenBehavior(typeof(MetricsBehaviour<,>));
            
            x.NotificationPublisher = new TaskWhenAllPublisher();
            x.NotificationPublisherType = typeof(TaskWhenAllPublisher);
        });
        
        return services;
    }
}