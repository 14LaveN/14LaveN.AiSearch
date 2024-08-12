using System.ComponentModel.DataAnnotations;

namespace Identity.API.Infrastructure.Settings.User;

/// <summary>
/// Represents jwt options class.
/// </summary>
public sealed class JwtOptions
{
    public const string SettingsKey = "Jwt";
    
    /// <summary>
    /// Gets or sets secret.
    /// </summary>
    [Required]
    public string Secret { get; set; } = null!;

    /// <summary>
    /// Gets or sets string list valid audiences.
    /// </summary>
    [Required]
    public List<string> ValidAudiences { get; init; } = null!;
    
    /// <summary>
    /// Gets or sets string list valid issuers.
    /// </summary>
    [Required]
    public List<string> ValidIssuers { get; init; } = null!;
    
    /// <summary>
    /// Gets or sets expire.
    /// </summary>
    public int Expire { get; set; } = 3600;
    
    /// <summary>
    /// Gets or sets refresh token expire.
    /// </summary>
    public int RefreshTokenExpire { get; set; } = 20160;
}