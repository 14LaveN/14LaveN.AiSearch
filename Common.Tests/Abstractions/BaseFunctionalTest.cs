using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Common.Tests.Abstractions;

namespace Common.Tests.Abstractions;

/// <summary>
/// Represents the base function test class.
/// </summary>
public abstract class BaseFunctionalTest : IDisposable
{
    /// <summary>
    /// Initialize the instance of <see cref="BaseFunctionalTest"/>.
    /// </summary>
    protected BaseFunctionalTest()
    {
        Fixture = new Fixture();
        using var webApplicationFactory = new WebApplicationFactory<Program>();
        HttpClient = webApplicationFactory.CreateClient();
    }

    /// <summary>
    /// Gets HttpClient.
    /// </summary>
    protected readonly HttpClient HttpClient;

    /// <summary>
    /// Gets Fixture.
    /// </summary>
    protected readonly Fixture Fixture;

    /// <inheritdoc/>
    public void Dispose()
    {
        HttpClient.Dispose();
    }
}