using Newtonsoft.Json;
using Application.Core.Abstractions.Messaging;
using Identity.Domain.Events.User;

namespace Identity.API.IntegrationEvents.User.Events.UserCreated;

/// <summary>
/// Represents the integration event that is raised when a user is created.
/// </summary>
public sealed class UserCreatedIntegrationEvent
    : IIntegrationEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserCreatedIntegrationEvent"/> class.
    /// </summary>
    /// <param name="userCreatedDomainEvent">The user created domain event.</param>
    public UserCreatedIntegrationEvent(UserCreatedDomainEvent userCreatedDomainEvent) => 
        Id = userCreatedDomainEvent.User.Id;
        
    [JsonConstructor]
    public UserCreatedIntegrationEvent(Ulid userId) => Id = userId;

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Ulid Id { get; }
}