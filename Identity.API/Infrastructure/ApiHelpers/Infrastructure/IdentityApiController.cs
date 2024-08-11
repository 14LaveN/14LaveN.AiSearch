using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.ApiHelpers.Contracts;
using Domain.Common.Core.Primitives;
using Domain.Common.Core.Primitives.Maybe;
using Domain.Common.Core.Primitives.Result;
using Domain.Core.Primitives.Result;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;

namespace Identity.Infrastructure.ApiHelpers.Infrastructure;

/// <summary>
/// Represents the identity api controller class.
/// </summary>
[ApiController]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "v1")]
public class IdentityApiController 
    : ControllerBase
{
    /// <summary>
    /// Initialize the instance of <see cref="IdentityApiController"/>.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="userRepository">The user repository.</param>
    protected IdentityApiController(
        ISender sender,
        IUserRepository userRepository)
    {
        Sender = sender;
        UserRepository = userRepository;
    }

    /// <summary>
    /// Gets sender.
    /// </summary>
    protected ISender Sender { get; }

    /// <summary>
    /// Gets the user repository.
    /// </summary>
    protected IUserRepository UserRepository { get; }

    /// <summary>
    /// Get the profile by identifier.
    /// </summary>
    /// <param name="authorId">The author identifier.</param>
    /// <returns></returns>
    [HttpGet("get-profile-by-id/{authorId}")]
    public async Task<Maybe<User>> GetProfileById([FromRoute] Guid authorId)
    {
        var profile = 
            await UserRepository.GetByIdAsync(authorId);

        return profile;
    }
        
    /// <summary>
    /// Creates an <see cref="BadRequestObjectResult"/> that produces a <see cref="StatusCodes.Status400BadRequest"/>.
    /// response based on the specified <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>The created <see cref="BadRequestObjectResult"/> for the response.</returns>
    protected IActionResult BadRequest(Error error) => BadRequest(new ApiErrorResponse(new[] { error }));
    
    /// <summary>
    /// Creates an <see cref="BadRequestObjectResult"/> that produces a <see cref="StatusCodes.Status400BadRequest"/>.
    /// response based on the specified <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>The created <see cref="BadRequestObjectResult"/> for the response.</returns>
    protected IActionResult Unauthorized(Error error) => Unauthorized(new ApiErrorResponse(new[] { error }));

    /// <summary>
    /// Creates an <see cref="OkObjectResult"/> that produces a <see cref="StatusCodes.Status200OK"/>.
    /// </summary>
    /// <returns>The created <see cref="OkObjectResult"/> for the response.</returns>
    /// <returns></returns>
    protected new IActionResult Ok(object value) => base.Ok(value);

    /// <summary>
    /// Creates an <see cref="NotFoundResult"/> that produces a <see cref="StatusCodes.Status404NotFound"/>.
    /// </summary>
    /// <returns>The created <see cref="NotFoundResult"/> for the response.</returns>
    protected new IActionResult NotFound() => base.NotFound();
}