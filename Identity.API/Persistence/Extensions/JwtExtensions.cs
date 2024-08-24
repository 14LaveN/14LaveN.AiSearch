using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Identity.API.Infrastructure.Settings.User;
using Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Application.Core.Abstractions;
using Domain.Entities;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure.Authentication;

namespace Identity.API.Persistence.Extensions;

/// <summary>
/// Represents the json web token extensions class.
/// </summary>
internal static class JwtExtensions
{
    /// <summary>
    /// Generate new refresh token by options.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="jwtOptions">The json web token options.</param>
    /// <returns>Returns refresh token.</returns>
    public static (string, DateTime) GenerateRefreshToken(
        this User user,
        JwtOptions jwtOptions)
    {
        var refreshTokenExpireAt = DateTime.UtcNow.AddMinutes(jwtOptions.RefreshTokenExpire);
        var data = new RefreshTokenData
        {
            Expire = refreshTokenExpireAt, 
            UserId = user.Id, 
            Key = StringRandomizer.Randomize()
        };
        
        return (AesEncryptor.Encrypt(jwtOptions.Secret, JsonSerializer.Serialize(data)), refreshTokenExpireAt);
    }

    /// <summary>
    /// Generate new access token by options.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="claims">The claims.</param>
    /// <param name="jwtOptions">The json web token options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns access token.</returns>
    public static Task<string> GenerateAccessToken(
        this User user,
        IEnumerable<Claim> claims,
        JwtOptions jwtOptions,
        CancellationToken cancellationToken = default)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret.PadRight(64)));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
        
        if (user.UserName is not null)
        {
            var tokeOptions = new JwtSecurityToken(
                jwtOptions.ValidIssuers.Last(),
                jwtOptions.ValidAudiences.Last(),
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtOptions.Expire),
                signingCredentials: signinCredentials
            );
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(tokeOptions));
        }

        throw new InvalidOperationException();
    }
}