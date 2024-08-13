using FluentValidation;
using Infrastructure.Authentication;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Application.ApiHelpers.Contracts;
using Application.ApiHelpers.Policy;
using Application.Core.Abstractions;
using Application.Core.Abstractions.Endpoint;
using Application.Core.Abstractions.Helpers.JWT;
using Application.Core.Abstractions.Messaging;
using Application.Core.Errors;
using Application.Core.Extensions;
using Domain.Common.Core.Errors;
using Domain.Common.Core.Primitives.Maybe;
using Domain.Common.Core.Primitives.Result;
using Domain.Common.ValueObjects;
using Domain.Core.Primitives.Result;
using Domain.Enumerations;
using Identity.API.Domain.Repositories;
using Identity.Domain.Entities;

namespace Identity.API.Mediatr.Commands;

/// <summary>
/// Represents the change password static class.
/// </summary>
public static class ChangePassword
{
    /// <summary>
    /// Represents the change password command record class.
    /// </summary>
    /// <param name="Password">The password.</param>
    public sealed record Command(string Password) 
        : ICommand<Result<User>>;
    
    /// <summary>
    /// Represents the change password <see cref="IEndpoint"/> class.
    /// </summary>
    public sealed class ChangePasswordEndpoint
        : IEndpoint
    {
        /// <inheritdoc />
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut(ApiRoutes.Users.ChangePassword, async (
                    [FromBody] string password,
                    ISender sender) =>
                {
                    var result = await Result.Create(password, DomainErrors.General.UnProcessableRequest)
                        .Map(changePasswordRequest => new Command(changePasswordRequest))
                        .Bind(command => BaseRetryPolicy.Policy.Execute(async () =>
                            await sender.Send(command)));

                    return result;
                })
                .HasPermission(Permission.ReadMember.ToString())
                .Produces(StatusCodes.Status401Unauthorized, typeof(ApiErrorResponse))
                .Produces(StatusCodes.Status200OK)
                .RequireRateLimiting("fixed");
        }
    }
    
    /// <summary>
    /// Represents the <see cref="Command"/> handler.
    /// </summary>
    internal sealed class CommandHandler
        : ICommandHandler<Command, Result<User>>
    {
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IUserIdentifierProvider _userIdentifier;
    
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="userIdentifier">The user identifier provider.</param>
        public CommandHandler(
            IUserUnitOfWork unitOfWork,
            UserManager<User> userManager,
            IUserIdentifierProvider userIdentifier)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _userIdentifier = userIdentifier;
        }
    
        /// <inheritdoc />
        public async Task<Result<User>> Handle(Command request, CancellationToken cancellationToken)
        {
            Result<Password> passwordResult = Password.Create(request.Password);
    
            if (passwordResult.IsFailure)
            {
                return Result.Failure<User>(passwordResult.Error);
            }
    
            Maybe<User> maybeUser = await _userManager.FindByIdAsync(_userIdentifier.UserId.ToString()) 
                                    ?? throw new ArgumentException();
    
            if (maybeUser.HasNoValue)
            {
                return  Result.Failure<User>(DomainErrors.User.NotFound);
            }
    
            User user = maybeUser.Value;
    
            var passwordHash = _userManager.PasswordHasher.HashPassword(user, passwordResult.Value);
    
            Result result = user.ChangePassword(passwordHash);
    
            if (result.IsFailure)
            {
                return Result.Failure<User>(result.Error);
            }
    
            await _unitOfWork.SaveChangesAsync(cancellationToken);
    
            return Result.Create(user, DomainErrors.General.ServerError);
        }
    }
    
    /// <summary>
    /// Represents the <see cref="Command"/> validator.
    /// </summary>
    internal sealed class CommandValidator : AbstractValidator<Command>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandValidator"/> class.
        /// </summary>
        public CommandValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithError(ValidationErrors.ChangePassword.PasswordIsRequired);
        }
    }
}