using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Persistence;
using Persistence;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Common.Tests.Abstractions;

/// <summary>
/// Represents the integration test web app factory class.
/// </summary>
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.0")
        .Build();
    
        //TODO Change container.
    private readonly PostgreSqlContainer _databaseContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("BSGenericDb")
        .WithUsername("postgres")
        .WithCommand("postgres")
        .Build();
    
    /// <summary>
    /// Initialize docker elements.
    /// </summary>
    public async Task InitializeAsync()
    {
        //await _databaseContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptorType =
                typeof(DbContextOptions<BaseDbContext>);

            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == descriptorType);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<BaseDbContext>(o => 
                o.UseNpgsql("Server=localhost;Port=5433;Database=ASGenericDb;User Id=postgres;Password=1111;", act 
                        =>
                    {
                        act.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        act.EnableRetryOnFailure(3);
                        act.CommandTimeout(30);
                    })
                    .LogTo(Console.WriteLine)
                    .EnableServiceProviderCaching()
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors());

            services.RemoveAll(typeof(RedisCacheOptions));

            services.AddStackExchangeRedisCache(options =>
                options.Configuration = _redisContainer.GetConnectionString());

            services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<Program>());
        });
    }

    /// <summary>
    /// Dispose docker elements.
    /// </summary>
    public new async Task DisposeAsync()
    {
        //await _databaseContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}