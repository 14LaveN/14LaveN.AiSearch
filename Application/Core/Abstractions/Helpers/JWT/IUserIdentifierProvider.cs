namespace Application.Core.Abstractions.Helpers.JWT;

/// <summary>
/// Represents the user identifier provider interface.
/// </summary>
public interface IUserIdentifierProvider
{
    /// <summary>
    /// Gets the authenticated user identifier.
    /// </summary>
    Ulid UserId { get; }
}