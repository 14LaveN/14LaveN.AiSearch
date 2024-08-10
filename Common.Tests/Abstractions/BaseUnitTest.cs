using AutoFixture;
using Bogus;
using Persistence;
using TeamTasks.Persistence;

namespace Common.Tests.Abstractions;

/// <summary>
/// Represents the base unit test class.
/// </summary>
public abstract class BaseUnitTest : IDisposable
{ 
    /// <summary>
    /// Initialize the instance of <see cref="BaseUnitTest"/>.
    /// </summary>
    protected BaseUnitTest()
    {
        DbContext = new BaseDbContext();
        Faker = new Faker();
        Fixture = new Fixture();
    }
    
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
    }
}