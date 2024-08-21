using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Core.Abstractions;
using Domain.Core.Exceptions;
using Domain.Entities;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure.Settings.User;
using Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Infrastructure.Authentication;

public static class CreateClaimsExtensions
{
    public static async Task<IEnumerable<Claim>> GenerateClaims(
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

        if (user.UserName is not null)
        {
            return claims.Union(new List<Claim>()
            {
                new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Name, user.UserName),
                new(JwtRegisteredClaimNames.Email, user.EmailAddress),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty)
            });
        }

        throw new NotFoundException(nameof(claims), claims);
    }
}