using Email.Contracts.Emails;

namespace Email.Emails;

/// <summary>
/// Represents the emailAddress notification service interface.
/// </summary>
public interface IEmailNotificationService
{
    /// <summary>
    /// Sends the welcome emailAddress notification based on the specified request.
    /// </summary>
    /// <param name="welcomeEmail">The welcome emailAddress.</param>
    /// <returns>The completed task.</returns>
    Task SendWelcomeEmail(WelcomeEmail welcomeEmail);
    
    /// <summary>
    /// Sends the password changed emailAddress.
    /// </summary>
    /// <param name="passwordChangedEmail">The password changed emailAddress.</param>
    /// <returns>The completed task.</returns>
    Task SendPasswordChangedEmail(PasswordChangedEmail passwordChangedEmail);
}