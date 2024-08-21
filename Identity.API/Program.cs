using System.Net;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using EmailService;
using Application;
using Application.ApiHelpers.Configurations;
using Application.ApiHelpers.Middlewares;
using Application.ApiHelpers.Middlewares.DelegatingHandlers;
using AspNetCore.Serilog.RequestLoggingMiddleware;
using Common.Logging;
using HealthChecks.UI.Client;
using Identity.Api.Common.DependencyInjection;
using Identity.API.Common.DependencyInjection;
using Identity.API.Components;
using Identity.API.Infrastructure;
using Identity.API.IntegrationEvents.User.Events.UserCreated;
using Identity.API.Persistence.Extensions;
using Identity.API.Refit.Users;
using Microsoft.AspNetCore.ResponseCompression;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Persistence;
using Prometheus;
using Prometheus.Client.AspNetCore;
using Prometheus.Client.HttpRequestDurations;
using RabbitMq;
using RabbitMq.Extensions;
using Refit;
using Serilog;
using ServiceDefaults;

#region BuilderRegion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddServiceDefaults();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
    options.AddPolicy("fixed", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name?.ToString(),
            factory: _ => 
                new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(10),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }));
});

builder.Services
    .AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
    })
    .Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = System.IO.Compression.CompressionLevel.Optimal;
    })
    .Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = System.IO.Compression.CompressionLevel.SmallestSize;
    });

builder.Services
    .AddMediatr()
    .AddEndpoints(typeof(DiMediator).Assembly);

builder.Services
    .AddEmailService(builder.Configuration)
    .AddInfrastructure()
    .AddIdentityInfrastructure()
    .AddDatabase(builder.Configuration, builder.Environment)
    .AddMetricsOpenTelemetry(builder.Logging)
    .AddSwagger();
    //TODO .AddBackgroundTasks(builder.Configuration)
    //TODO .AddCaching(builder.Configuration)

builder.Services
    .AddRabbitMq(builder.Configuration)
    .ConfigureJsonOptions(options => options.TypeInfoResolverChain.Add(IntegrationEventContext.Default));

builder.Host.UseSerilog(Logging.ConfigureLogger);

builder.Services.AddTransient<LogContextEnrichmentMiddleware>();

builder.Services.AddApplication();

builder.Services
    .AddAuthorizationExtension(builder.Configuration)
    .AddCors(options => options.AddDefaultPolicy(corsPolicyBuilder =>
        corsPolicyBuilder.WithOrigins("https://localhost:44442", "http://localhost:44460")
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5000, listenOptions =>
    {
        listenOptions.UseConnectionLogging();
    });
    
    options.Limits.MaxRequestBodySize = 10 * 1024; // 10 KB
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.MaxConcurrentConnections = 100;
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
});



var refitSettings = new RefitSettings
{
    // Настройка обработки ошибок
    ExceptionFactory = async response =>
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return new Exception($"API Error: {content}");
        }
        return null;
    }
};

builder.Services.AddTransient<LoggingHandler>();
builder.Services
    .AddRefitClient<IUsersClient>(refitSettings)
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5000"))
    .AddHttpMessageHandler<LoggingHandler>();


#endregion

#region ApplicationRegion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
        //app.UseSwaggerApp();
    app.ApplyUserDbMigrations();
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseRateLimiter();

app.UseCors();

UseMetrics();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
app.UseIdentityServer();

app.UseEndpoints(endpoints =>
{
   endpoints.MapHealthChecks("/health", new HealthCheckOptions
   {
       ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
   });
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.UseSerilogRequestLogging();

UseCustomMiddlewares();

MapEndpoints();

app.Run();

#endregion

#region UseMiddlewaresRegion
void UseCustomMiddlewares()
{
    if (app is null)
        throw new ArgumentException();

    app
        .UseMiddleware<IdempotentRequestMiddleware>()
        .UseMiddleware<RequestLoggingMiddleware>(app.Logger)
        //TODO .UseMiddleware<ResponseCachingMiddleware>()
        .UseMiddleware<LogContextEnrichmentMiddleware>();
    
}

void UseMetrics()
{
    if (app is null)
        throw new ArgumentException();
    
    app.MapMetrics();
    app.UseMetricServer();
    app.UseHttpMetrics();
    //app.UsePrometheusServer();
    //app.UsePrometheusRequestDurations();
}

void MapEndpoints()
{
    if (app is null)
        throw new ArgumentException();

    app.MapEndpoints();
}

#endregion

[JsonSerializable(typeof(UserCreatedIntegrationEvent))]
partial class IntegrationEventContext : JsonSerializerContext
{

}