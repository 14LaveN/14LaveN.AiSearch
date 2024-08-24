using System.Numerics;
using Domain.Common.ValueObjects;
using Domain.ValueObjects;
using Identity.API.Common;
using Identity.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Persistence;

namespace Identity.API.Persistence;

public sealed class UserDbContextSeed(
    ILogger<UserDbContextSeed> logger) 
    : IDbSeeder<UserDbContext>
{
    public async Task SeedAsync(UserDbContext context)
    {
        await context.Database.OpenConnectionAsync();
        await ((NpgsqlConnection)context.Database.GetDbConnection()).ReloadTypesAsync();

        if (!context.Set<User>().Any())
        {
            context.Set<User>().RemoveRange(context.Set<User>());
            await context.Set<User>().AddRangeAsync([
                new User(
                "hfdgdgg",
                FirstName.Create("dfdffsdfdsf").Value,
                LastName.Create("dfdsfdsfsd").Value,
                EmailAddress.Create("sasha@mail.ru").Value,
                "Sasha_2008!"),
                new User(
                    "hfdfdgdfg",
                    FirstName.Create("dfdffsdfdgdfgf").Value,
                    LastName.Create("dfdfgfdgdsfsd").Value,
                    EmailAddress.Create("sasha@mail.ru").Value,
                    "Sasha_2008!"),
                new User(
                    "hdfgfdgfdgdfg",
                    FirstName.Create("dfdfdgdfgf").Value,
                    LastName.Create("dfdfgfdgdsfsd").Value,
                    EmailAddress.Create("sasha@mail.ru").Value,
                    "Sasha_2008!")
            ]);
            logger.LogInformation(
                "Seeded users with {NumBrands}.", 
                context.Set<User>().Count());
            
            await context.SaveChangesAsync();
        }
    }
}
