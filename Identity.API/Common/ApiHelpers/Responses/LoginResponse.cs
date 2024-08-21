using System.Net;
using Domain.Core.Primitives.Result;
using Identity.API.Domain.Entities;

namespace Identity.API.Common.ApiHelpers.Responses;

/// <summary>
/// Gets or sets login response class.
/// </summary>
/// <typeparam name="T">The generic type.</typeparam>
public class LoginResponse<T> 
{
    /// <summary>
    /// Gets or sets description.
    /// </summary>
    public required string Description { get; set; } = null!;

    /// <summary>
    /// Gets or sets data.
    /// </summary>
    public Task<Result<User>> Data { get; set; }

    /// <summary>
    /// Gets or sets status code.
    /// </summary>
    public required HttpStatusCode StatusCode { get; set; }
    
    /// <summary>
    /// Gets or sets access token.
    /// </summary>
    public string AccessToken { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets date/time refresh token expire at.
    /// </summary>
    public DateTime RefreshTokenExpireAt { get; set; }
}