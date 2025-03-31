using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace KPISolution.Services.Email
{
    /// <summary>
    /// Cài đặt dịch vụ gửi email qua SMTP
    /// </summary>
    public class EmailSender : IEmailSenderExtended
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailSender> _logger;
        private readonly IHostEnvironment _environment;

        /// <summary>
        /// Tạo instance mới của EmailSender
        /// </summary>
        /// <param name="smtpSettings">Cấu hình SMTP</param>
        /// <param name="logger">Logger</param>
        /// <param name="environment">Environment</param>
        public EmailSender(
            IOptions<SmtpSettings> smtpSettings,
            ILogger<EmailSender> logger,
            IHostEnvironment environment)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
            _environment = environment;
        }

        /// <inheritdoc/>
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                // In development environment, only log the email instead of sending it
                if (_environment.IsDevelopment())
                {
                    _logger.LogInformation("DEVELOPMENT MODE - Email would be sent to {Email} with subject '{Subject}'", email, subject);
                    _logger.LogDebug("Email content: {Content}", message);
                    return; // Skip actual sending in development
                }

                var mail = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mail.To.Add(new MailAddress(email));

                using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.EnableSsl = _smtpSettings.UseSsl;

                    await client.SendMailAsync(mail);
                    _logger.LogInformation("Email sent successfully to {Email} with subject '{Subject}'", email, subject);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} with subject '{Subject}' - This is non-critical in development mode", email, subject);

                // Don't rethrow in development environment to allow registration/password reset to continue
                if (!_environment.IsDevelopment())
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gửi email xác nhận
        /// </summary>
        /// <param name="email">Email của người nhận</param>
        /// <param name="callbackUrl">URL để xác nhận</param>
        /// <returns>Task đại diện cho quá trình gửi email bất đồng bộ</returns>
        public async Task SendConfirmationEmailAsync(string email, string callbackUrl)
        {
            var subject = "Xác nhận tài khoản";
            var message = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; color: #333; }}
                        .container {{ padding: 20px; }}
                        .button {{ background-color: #4CAF50; border: none; color: white; padding: 10px 20px; text-align: center; 
                                  text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer; border-radius: 4px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Xác nhận tài khoản của bạn</h2>
                        <p>Cảm ơn bạn đã đăng ký tài khoản trong Hệ thống Quản lý KPI.</p>
                        <p>Vui lòng xác nhận tài khoản của bạn bằng cách nhấp vào nút dưới đây:</p>
                        <p><a href='{callbackUrl}' class='button'>Xác nhận tài khoản</a></p>
                        <p>Nếu nút trên không hoạt động, vui lòng copy liên kết sau và dán vào trình duyệt:</p>
                        <p>{callbackUrl}</p>
                        <p>Trân trọng,<br>Đội ngũ Hệ thống Quản lý KPI</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, message);
        }

        /// <summary>
        /// Gửi email đặt lại mật khẩu
        /// </summary>
        /// <param name="email">Email của người nhận</param>
        /// <param name="callbackUrl">URL để đặt lại mật khẩu</param>
        /// <returns>Task đại diện cho quá trình gửi email bất đồng bộ</returns>
        public async Task SendPasswordResetEmailAsync(string email, string callbackUrl)
        {
            var subject = "Đặt lại mật khẩu";
            var message = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; color: #333; }}
                        .container {{ padding: 20px; }}
                        .button {{ background-color: #4CAF50; border: none; color: white; padding: 10px 20px; text-align: center; 
                                  text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer; border-radius: 4px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Đặt lại mật khẩu</h2>
                        <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                        <p>Vui lòng nhấp vào nút dưới đây để đặt lại mật khẩu:</p>
                        <p><a href='{callbackUrl}' class='button'>Đặt lại mật khẩu</a></p>
                        <p>Nếu nút trên không hoạt động, vui lòng copy liên kết sau và dán vào trình duyệt:</p>
                        <p>{callbackUrl}</p>
                        <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này hoặc liên hệ với quản trị viên.</p>
                        <p>Trân trọng,<br>Đội ngũ Hệ thống Quản lý KPI</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, message);
        }

        /// <summary>
        /// Gửi email thông báo
        /// </summary>
        /// <param name="email">Email của người nhận</param>
        /// <param name="subject">Tiêu đề thông báo</param>
        /// <param name="message">Nội dung thông báo</param>
        /// <returns>Task đại diện cho quá trình gửi email bất đồng bộ</returns>
        public async Task SendNotificationEmailAsync(string email, string subject, string message)
        {
            var htmlMessage = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; color: #333; }}
                        .container {{ padding: 20px; }}
                        .notification {{ background-color: #f2f2f2; padding: 15px; border-left: 4px solid #4CAF50; margin-bottom: 15px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Thông báo từ Hệ thống Quản lý KPI</h2>
                        <div class='notification'>
                            <h3>{subject}</h3>
                            <p>{message}</p>
                        </div>
                        <p>Trân trọng,<br>Đội ngũ Hệ thống Quản lý KPI</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, htmlMessage);
        }
    }
}