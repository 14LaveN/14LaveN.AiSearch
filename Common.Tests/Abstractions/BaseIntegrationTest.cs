using AutoFixture;
using Bogus;
using Common.Tests.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Persistence;
using Common.Tests.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Persistence;

namespace Common.Tests.Abstractions;

/// <summary>
/// Represents the base integration test class.
/// </summary>
public abstract class BaseIntegrationTest<T>
    : IClassFixture<T>, IDisposable where T : WebApplicationFactory<Program>
{ 
    private readonly IServiceScope _scope;

    /// <summary>
    /// Initialize the instance of <see cref="T"/>.
    /// </summary>
    /// <param name="factory">The integration test web app factory.</param>
    protected BaseIntegrationTest(T factory)
    {
        _scope = factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<BaseDbContext>();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        Faker = new Faker();
        Fixture = new Fixture();
    }

    /// <summary>
    /// Gets mediatr sender.
    /// </summary>
    protected readonly ISender Sender;

    /// <summary>
    /// Gets Base database context.
    /// </summary>
    protected readonly BaseDbContext DbContext;

    /// <summary>
    /// Gets Fixture.
    /// </summary>
    protected readonly Fixture Fixture;

    /// <summary>
    /// Gets Faker.
    /// </summary>
    protected readonly Faker Faker;

    /// <inheritdoc/>
    public void Dispose()
    {
        DbContext.Dispose();
        _scope.Dispose();
    }
}