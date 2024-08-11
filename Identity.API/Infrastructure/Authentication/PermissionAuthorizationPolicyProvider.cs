using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Domain.Entities;

namespace Identity.API.Infrastructure.Authentication;

/// <summary>
/// Represents the <see cref="Permission"/> authorization policy provider class.
/// </summary>
/// <param name="options">The authorization options policy</param>
public sealed class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    /// <inheritdoc />
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

        if (policy is not null)
        {
            return policy;
        }

        return new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
    }
}