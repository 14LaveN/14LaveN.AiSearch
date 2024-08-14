using Domain.Common.Core.Errors;
using Domain.Common.Core.Exceptions;
using Domain.Common.Core.Primitives.Maybe;
using Email.Contracts.Emails;
using Email.Emails;
using Identity.API.Domain.Repositories;
using Identity.API.IntegrationEvents.User.Events.UserCreated;
using RabbitMq.Abstractions;

namespace Identity.API.IntegrationEvents.User.Handlers.UserCreated;

/// <summary>
/// Represents the <see cref="UserCreatedIntegrationEvent"/> handler.
/// </summary>
internal sealed class SendWelcomeEmailOnUserCreatedIntegrationEventHandler 
    : IIntegrationEventHandler<UserCreatedIntegrationEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailNotificationService _emailNotificationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendWelcomeEmailOnUserCreatedIntegrationEventHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="emailNotificationService">The email notification service.</param>
    public SendWelcomeEmailOnUserCreatedIntegrationEventHandler(
        IUserRepository userRepository,
        IEmailNotificationService emailNotificationService)
    {
        _emailNotificationService = emailNotificationService;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task Handle(UserCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        Maybe<Domain.Entities.User> maybeUser = await _userRepository.GetByIdAsync(notification.Id);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        Domain.Entities.User user = maybeUser.Value;

        var welcomeEmail = new WelcomeEmail(user.Email, user.FullName);

        await _emailNotificationService.SendWelcomeEmail(welcomeEmail);
    }
}