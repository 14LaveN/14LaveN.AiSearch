using Microsoft.AspNetCore.Authorization;

namespace Identity.API.Infrastructure.Authentication;

/// <summary>
/// Represents the permission requirement class.
/// </summary>
/// <param name="Permission">The permission.</param>
public sealed record PermissionRequirement(string Permission) 
    : IAuthorizationRequirement;