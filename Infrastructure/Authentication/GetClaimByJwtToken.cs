using System.IdentityModel.Tokens.Jwt;
using Infrastructure.Authentication;

namespace Infrastructure.Authentication;

/// <summary>
/// Represents the Get claim by jwt token class.
/// </summary>
public static class GetClaimByJwtToken
{
    /// <summary>
    /// Get name by JWT.
    /// </summary>
    /// <param name="token">The JWT.</param>
    /// <returns>Return the name from token.</returns>
    public static string GetNameByToken(string? token)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken tokenInfo = handler.ReadJwtToken(token);
        
        var claimsPrincipal = tokenInfo.Claims;
        
        var name = claimsPrincipal.FirstOrDefault(x=> x.Type == "name")?.Value;
        return name!;
    }
    
    /// <summary>
    /// Get permissions by JWT.
    /// </summary>
    /// <param name="token">The JWT.</param>
    /// <returns>Return the permissions from token.</returns>
    public static HashSet<string> GetPermissionsByToken(string? token)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken tokenInfo = handler.ReadJwtToken(token);
        
        var claimsPrincipal = tokenInfo.Claims;
        
        HashSet<string> permissions = claimsPrincipal
            .Where(x => x.Type == CustomClaims.Permissions)
            .Select(x => x.Value)
            .ToHashSet();
        
        return permissions!;
    }
    
    /// <summary>
    /// Get identifier by JWT.
    /// </summary>
    /// <param name="token">The JWT.</param>
    /// <returns>Return the identifier from token.</returns>
    public static string? GetIdByToken(string? token)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken tokenInfo = handler.ReadJwtToken(token);
        
        var claimsPrincipal = tokenInfo.Claims;
        
        var name = claimsPrincipal.FirstOrDefault(x=> x.Type == "nameid")?.Value;
        return name!;
    }
}