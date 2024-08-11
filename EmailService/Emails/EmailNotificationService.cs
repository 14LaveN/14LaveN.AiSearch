using EmailService.Emails;
using Email.Contracts.Emails;

namespace Email.Emails;

/// <summary>
/// Represents the email notification service.
/// </summary>
internal sealed class EmailNotificationService : IEmailNotificationService
{
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailNotificationService"/> class.
    /// </summary>
    /// <param name="emailService">The email service.</param>
    public EmailNotificationService(IEmailService emailService) => 
        _emailService = emailService;

    /// <inheritdoc />
    public async Task SendWelcomeEmail(WelcomeEmail welcomeEmail)
    {
        var mailRequest = new MailRequest(
            welcomeEmail.EmailTo,
            "Welcome to Event Reminder! 🎉",
            $"Welcome to Event Reminder {welcomeEmail.Name}," +
            Environment.NewLine +
            Environment.NewLine +
            $"You have registered with the email {welcomeEmail.EmailTo}.");

        await _emailService.SendEmailAsync(mailRequest);
    }
    /// <inheritdoc />
    public async Task SendPasswordChangedEmail(PasswordChangedEmail passwordChangedEmail)
    {
        var mailRequest = new MailRequest(
            passwordChangedEmail.EmailTo,
            "Password changed 🔐",
            $"Hello {passwordChangedEmail.Name}," +
            Environment.NewLine +
            Environment.NewLine +
            "Your password was successfully changed.");

        await _emailService.SendEmailAsync(mailRequest);
    }

    /// <inheritdoc />
    public async Task SendNotificationEmail(NotificationEmail notificationEmail)
    {
        var mailRequest = new MailRequest(notificationEmail.EmailTo, notificationEmail.Subject, notificationEmail.Body);

        await _emailService.SendEmailAsync(mailRequest);
    }
}