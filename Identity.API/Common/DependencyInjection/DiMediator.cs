using Application.Core.Behaviours;
using Domain.Core.Utility;
using FluentValidation;
using Identity.API.IntegrationEvents.User.Events.PasswordChanged;
using Identity.API.IntegrationEvents.User.Events.UserCreated;
using Identity.API.IntegrationEvents.User.Handlers.PasswordChanged;
using Identity.API.IntegrationEvents.User.Handlers.UserCreated;
using Identity.API.Mediatr.Behaviour;
using Identity.API.Mediatr.Commands;
using Identity.API.Mediatr.Queries.GetTheUserById;
using Identity.Domain.Events.User;
using MediatR.NotificationPublishers;

namespace Identity.API.Common.DependencyInjection;

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

            x.RegisterServicesFromAssemblies(typeof(Register.Command).Assembly,
                    typeof(Register.CommandHandler).Assembly)
                .RegisterServicesFromAssemblies(typeof(Login.Command).Assembly,
                    typeof(Login.CommandHandler).Assembly)
                .RegisterServicesFromAssemblies(typeof(ChangePassword.Command).Assembly,
                    typeof(ChangePassword.CommandHandler).Assembly)
                .RegisterServicesFromAssemblies(typeof(ChangeName.Command).Assembly,
                    typeof(ChangeName.CommandHandler).Assembly)
                .RegisterServicesFromAssemblies(typeof(GetTheUserByIdQuery).Assembly,
                    typeof(GetTheUserByIdQueryHandler).Assembly)
                .RegisterServicesFromAssemblies(typeof(UserCreatedDomainEvent).Assembly,
                    typeof(PublishIntegrationEventOnUserCreatedDomainEventHandler).Assembly)
                .RegisterServicesFromAssemblies(typeof(UserCreatedIntegrationEvent).Assembly,
                    typeof(SendWelcomeEmailOnUserCreatedIntegrationEventHandler).Assembly)
                .RegisterServicesFromAssemblies(typeof(UserPasswordChangedIntegrationEvent).Assembly,
                    typeof(NotifyUserOnPasswordChangedIntegrationEventHandler).Assembly);
            
            x.AddOpenBehavior(typeof(QueryCachingBehavior<,>))
                //TODO .AddOpenBehavior(typeof(IdentityIdempotentCommandPipelineBehavior<,>))
                .AddOpenBehavior(typeof(UserTransactionBehavior<,>))
                .AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>))
                .AddOpenBehavior(typeof(ValidationBehaviour<,>))
                .AddOpenBehavior(typeof(MetricsBehaviour<,>));
            
            x.NotificationPublisher = new TaskWhenAllPublisher();
            x.NotificationPublisherType = typeof(TaskWhenAllPublisher);
        });
        
        return services;
    }
}