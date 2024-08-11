using Microsoft.AspNetCore.Http;
using Application.Core.Abstractions.Helpers.JWT;
using Infrastructure.Authentication;

namespace Infrastructure.Authentication;

public sealed class PermissionProvider : IPermissionProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public PermissionProvider(
        IHttpContextAccessor httpContextAccessor)
    {
        HashSet<string> permissions =  GetClaimByJwtToken
            .GetPermissionsByToken(httpContextAccessor
                .HttpContext?
                .Request
                .Headers["Authorization"]
            .FirstOrDefault()?
                .Split(" ")
                .Last());
        
        Permissions = permissions;
    }

    /// <inheritdoc />
    public HashSet<string> Permissions { get; }
}