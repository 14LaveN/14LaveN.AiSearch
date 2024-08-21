using Application.ApiHelpers.Contracts;
using Domain.Core.Primitives.Result;
using Identity.API.Contracts.Register;
using Identity.API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Identity.API.Common.Refit.Users;

/// <summary>
/// Represents the users client interface.
/// </summary>
public interface IUsersClient
{
    //TODO In Notion information How Create IntegrationTest with Refit.
    
    /// <summary>
    /// The register controller component.
    /// </summary>
    /// <param name="request">The <see cref="RegisterRequest"/>.</param>
    /// <param name="requestId">The request identifier.</param>
    /// <returns>Returns the result of <see cref="User"/> entity.</returns>
    [Post("/api/v1/"+ApiRoutes.Users.Register)]
    Task<Result<User>?> Register(
        [FromBody] RegisterRequest request,
        [FromHeader(Name = "X-Idempotency-Key")] string requestId);
}