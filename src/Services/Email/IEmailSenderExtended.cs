using Microsoft.AspNetCore.Identity.UI.Services;

namespace KPISolution.Services.Email
{
    /// <summary>
    /// Interface mở rộng từ IEmailSender với các chức năng bổ sung
    /// </summary>
    public interface IEmailSenderExtended : IEmailSender
    {
        /// <summary>
        /// Gửi email xác nhận
        /// </summary>
        /// <param name="email">Email của người nhận</param>
        /// <param name="callbackUrl">URL để xác nhận</param>
        /// <returns>Task đại diện cho quá trình gửi email bất đồng bộ</returns>
        Task SendConfirmationEmailAsync(string email, string callbackUrl);

        /// <summary>
        /// Gửi email đặt lại mật khẩu
        /// </summary>
        /// <param name="email">Email của người nhận</param>
        /// <param name="callbackUrl">URL để đặt lại mật khẩu</param>
        /// <returns>Task đại diện cho quá trình gửi email bất đồng bộ</returns>
        Task SendPasswordResetEmailAsync(string email, string callbackUrl);

        /// <summary>
        /// Gửi email thông báo
        /// </summary>
        /// <param name="email">Email của người nhận</param>
        /// <param name="subject">Tiêu đề thông báo</param>
        /// <param name="message">Nội dung thông báo</param>
        /// <returns>Task đại diện cho quá trình gửi email bất đồng bộ</returns>
        Task SendNotificationEmailAsync(string email, string subject, string message);
    }
}