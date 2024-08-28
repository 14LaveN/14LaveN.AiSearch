using Application.Core.Abstractions.Helpers.JWT;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Authentication;

internal sealed class UserNameProvider
    : IUserNameProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserNameProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="environment">The environment.</param>
    public UserNameProvider(
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
            UserName = GetClaimByJwtToken
                .GetNameByToken(httpContextAccessor
                    .HttpContext?
                    .Request
                    .Headers
                    .Authorization
                    .FirstOrDefault()?
                    .Split(" ").Last());

        else 
            UserName = httpContextAccessor
                .HttpContext?
                .User
                .FindFirst("name")?.Value;
    }
    
    /// <inheritdoc />
    public string? UserName { get; }
}