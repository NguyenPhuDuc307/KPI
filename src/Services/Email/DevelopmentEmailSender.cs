namespace KPISolution.Services.Email
{
    /// <summary>
    /// Mock implementation of email sender for development environment
    /// </summary>
    public class DevelopmentEmailSender : IEmailSenderExtended
    {
        private readonly ILogger<DevelopmentEmailSender> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="DevelopmentEmailSender"/>
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public DevelopmentEmailSender(ILogger<DevelopmentEmailSender> logger)
        {
            this._logger = logger;
        }

        /// <inheritdoc/>
        public Task SendEmailAsync(string email, string subject, string message)
        {
            this._logger.LogInformation("DEVELOPMENT EMAIL: Would send email to {Email} with subject '{Subject}'", email, subject);
            this._logger.LogDebug("Email content: {Content}", message);

            // Simulate successful email sending
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SendConfirmationEmailAsync(string email, string callbackUrl)
        {
            this._logger.LogInformation("DEVELOPMENT EMAIL: Would send confirmation email to {Email}", email);
            this._logger.LogInformation("Confirmation URL: {Url}", callbackUrl);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SendPasswordResetEmailAsync(string email, string callbackUrl)
        {
            this._logger.LogInformation("DEVELOPMENT EMAIL: Would send password reset email to {Email}", email);
            this._logger.LogInformation("Password reset URL: {Url}", callbackUrl);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SendNotificationEmailAsync(string email, string subject, string message)
        {
            this._logger.LogInformation("DEVELOPMENT EMAIL: Would send notification email to {Email} with subject '{Subject}'", email, subject);
            this._logger.LogDebug("Notification content: {Content}", message);

            return Task.CompletedTask;
        }
    }
}
