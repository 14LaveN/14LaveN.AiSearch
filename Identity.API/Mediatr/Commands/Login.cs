using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using Application.ApiHelpers.Contracts;
using Application.ApiHelpers.Policy;
using Application.Core.Abstractions;
using Application.Core.Abstractions.Endpoint;
using Application.Core.Abstractions.Messaging;
using Domain.Common.Core.Errors;
using Domain.Common.Core.Primitives.Result;
using Domain.Common.ValueObjects;
using Domain.Core.Exceptions;
using Domain.Core.Primitives.Result;
using FluentValidation;
using Identity.API.Abstractions.Idempotency;
using Identity.API.ApiHelpers.Responses;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure.Authentication;
using Identity.API.Infrastructure.Settings.User;
using Identity.API.Persistence;
using Identity.API.Persistence.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Identity.API.Mediatr.Commands;

/// <summary>
/// Represents the login static class.
/// </summary>
public static class Login
{
    /// <summary>
    /// Represents the login command record class.
    /// </summary>
    /// <param name="UserName">The user name.</param>
    /// <param name="Password">The password.</param>
    /// <param name="RequestId">The request identifier.</param>
    public sealed record Command(
        Ulid RequestId,
        string UserName,
        Password Password)
        : IdentityIdempotentCommand(RequestId);
    
    /// <summary>
    /// Represents the login <see cref="IEndpoint"/> class.
    /// </summary>
    public sealed class LoginEndpoint
        : IEndpoint
    {
        /// <inheritdoc />
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{version:apiVersion}/" + ApiRoutes.Users.Login, async (
                    [FromBody] Contracts.Login.LoginRequest request,
                    [FromHeader(Name = "X-Idempotency-Key")] string requestId,
                    ISender sender) =>
                {
                    if (!Ulid.TryParse(requestId, out Ulid parsedRequestId))
                        throw   new UlidParseException(nameof(requestId), requestId);

                    var result = await Result.Create(request, DomainErrors.General.UnProcessableRequest)
                        .Map(loginRequest => new Command(
                            parsedRequestId,
                            loginRequest.UserName,
                            Password.Create(loginRequest.Password).Value))
                        .Bind(command => BaseRetryPolicy.Policy.Execute(async () =>
                            await sender.Send(command)).Result.Data);

                    return result;
                })
                .AllowAnonymous()
                .Produces(StatusCodes.Status401Unauthorized, typeof(ApiErrorResponse))
                .Produces(StatusCodes.Status200OK)
                .RequireRateLimiting("fixed");
        }
    }
    
    /// <summary>
    /// Represents the login command handler class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="jwtOptions">The json web token options.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="signInManager">The sign in manager.</param>
    internal sealed class CommandHandler(
            ILogger<CommandHandler> logger,
            UserManager<User> userManager,
            IOptions<JwtOptions> jwtOptions,
            SignInManager<User> signInManager,
            UserDbContext dbContext)
        : ICommandHandler<Command, LoginResponse<Result<User>>>
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
        private readonly SignInManager<User> _signInManager = signInManager ?? throw new ArgumentNullException();
        
        /// <inheritdoc />
        public async Task<LoginResponse<Result<User>>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            try
            {
                User? user = await userManager.FindByNameAsync(request.UserName);
    
                if (user is null)
                {
                    logger.LogWarning("User with the same name not found");
                    throw new NotFoundException(nameof(user), "User with the same name");
                }
    
                if (!await userManager.CheckPasswordAsync(user, request.Password))
                {
                    logger.LogWarning("The password does not meet the assessment criteria");
                    throw new AuthenticationException();
                }
                
                
                IEnumerable<Claim> claims = await user.GenerateClaims(dbContext, _jwtOptions, cancellationToken);
                
                var result = await _signInManager.PasswordSignInAsync(
                    request.UserName,
                    request.Password,
                    false,
                    false);
    
                var (refreshToken, refreshTokenExpireAt) = user.GenerateRefreshToken(_jwtOptions);
                
                if (result.Succeeded)
                {
                    user.RefreshToken = refreshToken;
                }
                
                logger.LogInformation($"User logged in - {user.UserName} {DateTime.UtcNow}");
                
                return new LoginResponse<Result<User>>
                {
                    Description = "Login account",
                    StatusCode = HttpStatusCode.OK,
                    Data =  Task.FromResult(Result.Create(user, DomainErrors.General.ServerError)),
                    AccessToken =  await user.GenerateAccessToken(claims, _jwtOptions, cancellationToken), 
                    RefreshToken = refreshToken,
                    RefreshTokenExpireAt = refreshTokenExpireAt
                };
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"[LoginCommandHandler]: {exception.Message}");
                throw new AuthenticationException(exception.Message);
            }
        }
    }
    
    /// <summary>
    /// Represents the login command validator class.
    /// </summary>
    internal sealed class CommandValidator
        : AbstractValidator<Command>
    {
        /// <summary>
        /// Validate the login command.
        /// </summary>
        public CommandValidator()
        {
            RuleFor(loginCommand =>
                    loginCommand.UserName).NotEqual(string.Empty)
                .WithMessage("You don't enter a user name")
                .MaximumLength(28)
                .WithMessage("Your user name is too big");
        
            RuleFor(loginCommand =>
                    loginCommand.Password.Value).NotEqual(string.Empty)
                .WithMessage("You don't enter a password")
                .MaximumLength(36)
                .WithMessage("Your password is too big");
        }
    }
}