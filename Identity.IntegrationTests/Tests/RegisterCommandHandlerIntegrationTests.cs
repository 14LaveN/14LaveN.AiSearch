using Common.Tests.Abstractions;
using Domain.Core.Primitives.Result;
using Identity.API.ApiHelpers.Responses;
using Identity.API.Domain.Entities;
using Identity.API.Mediatr.Commands;
using MediatR;

namespace Identity.IntegrationTests.Tests;

public sealed class RegisterCommandHandlerIntegrationTests(IdentityWebAppFactory factory)
    : BaseIntegrationTest<IdentityWebAppFactory>(factory)
{
    private readonly IdentityWebAppFactory _factory = factory;

    [Fact]
    public async Task Register_DataShouldBeNotNull()
    {
        var client = _factory.CreateClient();
        
        Register.Command command = Fixture.Create<Register.Command>();

        LoginResponse<Result<User>> response = await Sender.Send(command);

        response.Data.Should().NotBeNull();
    }
}