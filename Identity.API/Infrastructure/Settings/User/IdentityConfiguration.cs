using IdentityServer4;
using IdentityServer4.Models;

namespace Identity.API.Infrastructure.Settings.User;

/// <summary>
/// Represents the identity configuration class.
/// </summary>
public static class IdentityConfiguration
{
    /// <summary>
    /// The api scopes list.
    /// </summary>
    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new("identityApi", "Identity.Api")
        };

    /// <summary>
    /// The identity resources list.
    /// </summary>
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    /// <summary>
    /// The api resources list.
    /// </summary>
    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new("identityApi", "Identity.Api")
        };

    /// <summary>
    /// The clients list.
    /// </summary>
    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new()
            {
                ClientId = "team-tasks_identity",
                ClientName = "Identity.Api",
                AllowedGrantTypes = GrantTypes.Hybrid,
                RequireClientSecret = false,
                RequirePkce = false,
                RedirectUris =
                {
                    "http://localhost:7135"
                },
                AllowedCorsOrigins =
                {
                    "http://localhost:7135"
                },
                PostLogoutRedirectUris =
                {
                    "http://localhost:7135"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "identityApi"
                },
                AllowAccessTokensViaBrowser = true
            },
            new()
            {
                //TODO Change the uri.
                ClientId = "reactClient",
                ClientName = "React SPA Client",
                ClientUri = "https://localhost:44467",
                RequireConsent = false,
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                AllowAccessTokensViaBrowser = true,
                RedirectUris =
                {
                    "https://localhost:44467",
                    "https://localhost:44467/auth-callback",
                    "https://localhost:44467/silent-renew.html"
                },
                PostLogoutRedirectUris = new List<string>() { "https://localhost:44467" },
                AllowedCorsOrigins = { "https://localhost:44467" },
                AllowedScopes = new List<string>()
                { 
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile, 
                    "identityApi"
                }
            }
        };
}