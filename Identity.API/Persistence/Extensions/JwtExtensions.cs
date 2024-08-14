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
    /// <param name="dbContext">The database context.</param>
    /// <param name="jwtOptions">The json web token options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns access token.</returns>
    public static async Task<string> GenerateAccessToken(
        this User user,
        IDbContext dbContext,
        JwtOptions jwtOptions,
        CancellationToken cancellationToken = default)
    {
        Role? existingRole = await dbContext
            .Set<Role>()
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(r => r.Value == Role.Registered.Value, cancellationToken: cancellationToken);

        user.Roles ??= [];

        if (existingRole != null
            && user.Roles is not null
            && !user.Roles.Any(r => r.Value == existingRole.Value))
        {
            bool hasAllPermissions = existingRole.Permissions
                .All(permission =>
                    user.Roles
                        .SelectMany(r => r.Permissions)
                        .Any(p => p.Id == permission.Id));

            if (!hasAllPermissions)
            {
                user.Roles.Add(existingRole);
            }
        }

        List<Claim> claims = [];
        
        claims.AddRange(user.Roles!
            .First()
            .Permissions
            .ToList()
            .Select(permission =>
                new Claim(CustomClaims.Permissions, permission.Name)));

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret.PadRight(64)));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
        
        if (user.UserName is not null)
        {
            var tokeOptions = new JwtSecurityToken(
                jwtOptions.ValidIssuers.Last(),
                jwtOptions.ValidAudiences.Last(),
                claims: new List<Claim>
                {
                    new (JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                    new (JwtRegisteredClaimNames.Name, user.UserName),
                    new (JwtRegisteredClaimNames.Email, user.EmailAddress),
                    new (JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty),
                    new (JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty)
                }
                    .Union(claims),
                expires: DateTime.Now.AddMinutes(jwtOptions.Expire),
                signingCredentials: signinCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }

        throw new InvalidOperationException();
    }
}