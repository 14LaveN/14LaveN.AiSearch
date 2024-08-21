using Microsoft.AspNetCore.Http;
using Application.Core.Abstractions.Helpers.JWT;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Authentication;

internal sealed class PermissionProvider : IPermissionProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="environment">The environment.</param>
    public PermissionProvider(
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment environment)
    {
        HashSet<string> permissions = new HashSet<string>();
        
        if (environment.IsDevelopment())
            permissions = GetClaimByJwtToken
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