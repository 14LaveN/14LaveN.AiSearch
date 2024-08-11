namespace Identity.API.Contracts.Get;

/// <summary>
/// Represents the get user response record.
/// </summary>
/// <param name="FullName">The full name.</param>
/// <param name="UserName">The user name.</param>
/// <param name="EmailAddress">The email address.</param>
/// <param name="CreatedAt">The date/time creation.</param>
public sealed record GetUserResponse(
    string FullName,
    string UserName,
    string EmailAddress,
    DateTime CreatedAt);