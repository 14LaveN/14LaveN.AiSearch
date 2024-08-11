using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Domain.Core.Utility;

namespace Application.ApiHelpers.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwachbackleService(
        this IServiceCollection services,
        Assembly assembly,
        string apiName)
    {
        Ensure.NotNull(services, "Services is required.", nameof(services));

        return services.AddSwaggerGen(options =>
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Version = description.ApiVersion.ToString(),
                    Title = apiName,
                    Description = $"Backend Web API на C# .NET for {apiName} application {description.GroupName}",
                    Contact = new OpenApiContact
                    {
                        Name = "GitHub",
                        Url = new Uri("https://github.com/14LaveN")
                    }
                });
            }

            if (apiName is not "Identity.Api")
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header используется в расшифровке запроса.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            }

            var xmlFilename = $"{assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    public static IApplicationBuilder UseSwaggerApp(this IApplicationBuilder app)
    {
        if (app is null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();  
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
            options.RoutePrefix = string.Empty;
        });
        return app;
    }
}