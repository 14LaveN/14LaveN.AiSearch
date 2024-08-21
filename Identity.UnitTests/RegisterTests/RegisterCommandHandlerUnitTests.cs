using Application.Core.Abstractions;
using AutoFixture;
using Common.Tests.Abstractions;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure.Authentication.SignIn;
using Identity.API.Infrastructure.Settings.User;
using Identity.API.Mediatr.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Identity.UnitTests.RegisterTests;

public sealed class RegisterCommandHandlerUnitTests
    : BaseUnitTest
{
    private readonly NullLogger<Register.CommandHandler> _logger;
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<SignInProvider<User>> _signInManager;
    private readonly Mock<IOptions<JwtOptions>> _jwtOptions;
    private readonly Mock<IDbContext> _dbContext;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

    private readonly Register.CommandHandler _commandHandler;

    public RegisterCommandHandlerUnitTests()
    {
        _userManager = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object, 
            null, null, null, null, null, null, null, null);
        _signInManager = new Mock<SignInProvider<User>>();
        _jwtOptions = new Mock<IOptions<JwtOptions>>();
        _dbContext = new Mock<IDbContext>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();

        _commandHandler = new Register.CommandHandler(
            _logger,
            _userManager.Object,
            _signInManager.Object,
            _jwtOptions.Object,
            _dbContext.Object,
            null,
            _httpContextAccessor.Object);
    }

    [Fact]
    public async Task RegisterCommandHandler_DataShouldBeNotNull()
    {
        Register.Command command = Fixture.Create<Register.Command>();

        var result = await _commandHandler.Handle(command, default);
        
        
    } 
}