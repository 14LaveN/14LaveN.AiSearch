namespace Identity.API.Infrastructure.Settings.User;

/// <summary>
/// Represents the refresh token data.
/// </summary>
public sealed class RefreshTokenData
{
    /// <summary>
    /// Gets or sets key.
    /// </summary>
    public string Key { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets date/time expire.
    /// </summary>
    public DateTime Expire { get; set; }
    
    /// <summary>
    /// Gets or sets user identifier.
    /// </summary>
    public Ulid UserId { get; set; }
}