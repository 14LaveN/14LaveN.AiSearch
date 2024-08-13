namespace ServiceDefaults;

public static class AuthenticationExtensions
{
//TODO     public static IServiceCollection AddDefaultAuthentication(this IHostApplicationBuilder builder)
//TODO     {
//TODO         var services = builder.Services;
//TODO         var configuration = builder.Configuration;
//TODO 
//TODO         // {
//TODO         //   "Identity": {
//TODO         //     "Url": "http://identity",
//TODO         //     "Audience": "basket"
//TODO         //    }
//TODO         // }
//TODO 
//TODO         var identitySection = configuration.GetSection("Identity");
//TODO 
//TODO         if (!identitySection.Exists())
//TODO         {
//TODO             // No identity section, so no authentication
//TODO             return services;
//TODO         }
//TODO 
//TODO         // prevent from mapping "sub" claim to nameidentifier.
//TODO         JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
//TODO 
//TODO         services.AddAuthentication().AddJwtBearer(options =>
//TODO         {
//TODO             var identityUrl = identitySection.GetRequiredValue("Url");
//TODO             var audience = identitySection.GetRequiredValue("Audience");
//TODO 
//TODO             options.Authority = identityUrl;
//TODO             options.RequireHttpsMetadata = false;
//TODO             options.Audience = audience;
//TODO             
//TODO #if DEBUG
//TODO             //Needed if using Android Emulator Locally. See https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/local-web-services?view=net-maui-8.0#android
//TODO             options.TokenValidationParameters.ValidIssuers = [identityUrl, "https://10.0.2.2:5243"];
//TODO #else
//TODO             options.TokenValidationParameters.ValidIssuers = [identityUrl];
//TODO #endif
//TODO             
//TODO             options.TokenValidationParameters.ValidateAudience = false;
//TODO         });
//TODO 
//TODO         services.AddAuthorization();
//TODO 
//TODO         return services;
//TODO     }
}
