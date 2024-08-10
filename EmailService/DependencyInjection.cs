using EmailService.Emails;
using TeamTasks.Email.Emails;
using TeamTasks.Email.Emails.Settings;

namespace EmailService;

public static class DependencyInjection
{
    public static IServiceCollection AddEmailService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null)
            throw new ArgumentException();
    
        services.Configure<MailSettings>(configuration.GetSection(MailSettings.SettingsKey));
        
        services.AddOptions<MailSettings>()
            .BindConfiguration(MailSettings.SettingsKey)
            .ValidateOnStart();
        
        services.AddScoped<IEmailService, EmailService.Emails.EmailService>();
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        
        return services;
    }
}