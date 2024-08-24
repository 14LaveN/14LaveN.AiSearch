using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ServiceDefaults;

public static class AuthenticationExtensions
{
     public static IServiceCollection AddDefaultAuthentication(this IHostApplicationBuilder builder)
     {
         var services = builder.Services;
         var configuration = builder.Configuration;
 
         ArgumentNullException.ThrowIfNull(services);
        
         services.AddAuthentication(config =>
             {
                 config.DefaultAuthenticateScheme =
                     JwtBearerDefaults.AuthenticationScheme;
                 
                 config.DefaultChallengeScheme = 
                     JwtBearerDefaults.AuthenticationScheme;
                 
                 config.DefaultScheme =
                     CookieAuthenticationDefaults.AuthenticationScheme;
             })
             .AddJwtBearer(options =>
             {
                 options.SaveToken = true;
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidIssuers = [configuration["Jwt:ValidIssuers"]],
                     ValidAudiences = new List<string>
                         {"https://localhost:7135", configuration["Jwt:ValidAudiences"]!},
                     IssuerSigningKey =
                         new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
                 };
             });

         services.AddAuthorization();

         services.AddCors(options => options.AddDefaultPolicy(corsPolicyBuilder =>
         {
             corsPolicyBuilder
                 .WithOrigins("https://localhost:44460/", "http://localhost:44460/", "http://localhost:44460")
                 .AllowAnyHeader()
                 .AllowAnyMethod();
         }));
        
         return services;
     }
}
