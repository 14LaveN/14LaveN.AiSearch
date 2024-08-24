using Microsoft.AspNetCore.Http;
using Application.Core.Abstractions.Helpers.JWT;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Authentication;

/// <summary>
/// Represents the user identifier provider.
/// </summary>   
internal sealed class UserIdentifierProvider : IUserIdentifierProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdentifierProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="environment">The environment.</param>
    public UserIdentifierProvider(
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment environment)
    {
        string? userId;
        if (environment.IsDevelopment())
            userId = GetClaimByJwtToken
                        .GetIdByToken(httpContextAccessor
                            .HttpContext?
                            .Request
                            .Headers["Authorization"]
                            .FirstOrDefault()?
                            .Split(" ").Last());

        else 
            userId = httpContextAccessor
            .HttpContext?
            .User
            .FindFirst("nameId")?.Value;

        bool isParsed = Ulid.TryParse(userId, out Ulid id);

        if (isParsed)
            if (userId is not null) UserId = id;
    }

    /// <inheritdoc />
    public Ulid UserId { get; }
}