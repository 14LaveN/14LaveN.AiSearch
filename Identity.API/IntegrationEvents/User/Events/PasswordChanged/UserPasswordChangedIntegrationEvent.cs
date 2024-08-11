using Newtonsoft.Json;
using Application.Core.Abstractions.Messaging;
using Identity.Domain.Events.User;

namespace Identity.API.IntegrationEvents.User.Events.PasswordChanged;

/// <summary>
/// Represents the integration event that is raised when a user's password is changed.
/// </summary>
public sealed class UserPasswordChangedIntegrationEvent 
    : IIntegrationEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserPasswordChangedIntegrationEvent"/> class.
    /// </summary>
    /// <param name="userPasswordChangedDomainEvent">The user password changed domain event.</param>
    public UserPasswordChangedIntegrationEvent(UserPasswordChangedDomainEvent userPasswordChangedDomainEvent) =>
        Id = userPasswordChangedDomainEvent.User.Id;

    [JsonConstructor]
    private UserPasswordChangedIntegrationEvent(Guid userId) => Id = userId;

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid Id { get; }
}