using Identity.API.Mediatr.Commands;
using Identity.API.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Identity.IntegrationTests;

public sealed class IdentityWebAppFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.0")
        .Build();
    
    //private readonly PostgreSqlContainer _databaseContainer = new PostgreSqlBuilder()
    //    .WithImage("postgres:latest")
    //    .WithDatabase("ASGenericDb")
    //    .WithUsername("postgres")
    //    .WithCommand("postgres")
    //    .Build();
    
    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptorType =
                typeof(DbContextOptions<UserDbContext>);

            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == descriptorType);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<UserDbContext>(o => 
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

            services.AddMediatR(x =>
                x.RegisterServicesFromAssemblyContaining(typeof(Register.Command)));
            
            services.AddSingleton<IStartupFilter>(new AutoAuthorizeStartupFilter());
        });
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        //await _databaseContainer.StopAsync();
        await _redisContainer.StopAsync();
    }

    /// <summary>
    /// Initialize docker elements.
    /// </summary>
    public async Task InitializeAsync()
    {
        //await _databaseContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    private class AutoAuthorizeStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<AutoAuthorizeMiddleware>();
                next(builder);
            };
        }
    }
}
