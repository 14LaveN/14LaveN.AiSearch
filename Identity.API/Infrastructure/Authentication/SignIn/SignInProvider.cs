using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Domain.Common.Core.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Identity.API.Infrastructure.Authentication.SignIn;

/// <summary>
/// Represents the sign in provider class.
/// </summary>
public class SignInProvider<TUser>(
    IHttpContextAccessor contextAccessor,
    IUserClaimsPrincipalFactory<TUser> claimsFactory)
    where TUser : class, IAuditableEntity
{
    private HttpContext? _context;

    /// <summary>
    /// The <see cref="HttpContext"/> used.
    /// </summary>
    public HttpContext Context
    {
        get
        {
            var context = _context ?? contextAccessor?.HttpContext;
            if (context == null)
            {
                throw new InvalidOperationException("HttpContext must not be null.");
            }
            return context;
        }
    }
    
    /// <summary>
    /// The authentication scheme to sign in with. Defaults to <see cref="CookieAuthenticationDefaults.AuthenticationScheme"/>.
    /// </summary>
    public const string AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    
    /// <summary>
    /// The <see cref="IUserClaimsPrincipalFactory{TUser}"/> used.
    /// </summary>
    public IUserClaimsPrincipalFactory<TUser> ClaimsFactory { get; set; } = claimsFactory;

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> for the specified <paramref name="user"/>, as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user to create a <see cref="ClaimsPrincipal"/> for.</param>
    /// <returns>The task object representing the asynchronous operation, containing the ClaimsPrincipal for the specified user.</returns>
    public virtual async Task<ClaimsPrincipal> CreateUserPrincipalAsync(TUser user) => await ClaimsFactory.CreateAsync(user);
    
    /// <summary>
    /// Signs in the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to sign-in.</param>
    /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed.</param>
    /// <param name="authenticationMethod">Name of the method used to authenticate the user.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public virtual Task SignInAsync(
        TUser user,
        bool isPersistent,
        List<Claim> claims,
        string? authenticationMethod = null)
        => SignInAsync(user, new AuthenticationProperties { IsPersistent = isPersistent }, claims, authenticationMethod);

    /// <summary>
    /// Signs in the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to sign-in.</param>
    /// <param name="authenticationProperties">Properties applied to the login and authentication cookie.</param>
    /// <param name="claims"></param>
    /// <param name="authenticationMethod">Name of the method used to authenticate the user.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters", Justification = "Required for backwards compatibility")]
    public virtual Task SignInAsync(TUser user,
        AuthenticationProperties authenticationProperties,
        List<Claim> claims,
        string? authenticationMethod = null)
    {
        IList<Claim>? additionalClaims = Array.Empty<Claim>();
        if (authenticationMethod != null)
        {
            additionalClaims = new List<Claim>();
            additionalClaims.Add(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            additionalClaims = additionalClaims.Union(claims) as IList<Claim>;
        }
        return SignInWithClaimsAsync(user, authenticationProperties, additionalClaims);
    }

    /// <summary>
    /// Signs in the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to sign-in.</param>
    /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed.</param>
    /// <param name="additionalClaims">Additional claims that will be stored in the cookie.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public virtual Task SignInWithClaimsAsync(
        TUser user,
        bool isPersistent,
        IEnumerable<Claim>? additionalClaims)
        => SignInWithClaimsAsync(user, new AuthenticationProperties { IsPersistent = isPersistent }, additionalClaims);

    /// <summary>
    /// Signs in the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to sign-in.</param>
    /// <param name="authenticationProperties">Properties applied to the login and authentication cookie.</param>
    /// <param name="additionalClaims">Additional claims that will be stored in the cookie.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public virtual async Task SignInWithClaimsAsync(
        TUser user,
        AuthenticationProperties? authenticationProperties,
        IEnumerable<Claim>? additionalClaims)
    {
        var userPrincipal = await CreateUserPrincipalAsync(user);
        foreach (var claim in additionalClaims)
        {
            userPrincipal.Identities.First().AddClaim(claim);
        }
        await Context.SignInAsync(AuthenticationScheme,
            userPrincipal,
            authenticationProperties ?? new AuthenticationProperties());
        
        Context.User = userPrincipal;
    }
}