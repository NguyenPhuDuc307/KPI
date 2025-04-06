namespace KPISolution.Services.Email
{
    /// <summary>
    /// Cấu hình SMTP để gửi email
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// Địa chỉ server SMTP
        /// </summary>
        public string Server { get; init; } = string.Empty;

        /// <summary>
        /// Cổng SMTP
        /// </summary>
        public int Port { get; init; }

        /// <summary>
        /// Địa chỉ email nguồn
        /// </summary>
        public string SenderEmail { get; init; } = string.Empty;

        /// <summary>
        /// Tên người gửi
        /// </summary>
        public string SenderName { get; init; } = string.Empty;

        /// <summary>
        /// Tên người dùng cho xác thực SMTP
        /// </summary>
        public string Username { get; init; } = string.Empty;

        /// <summary>
        /// Mật khẩu cho xác thực SMTP
        /// </summary>
        public string Password { get; init; } = string.Empty;

        /// <summary>
        /// Có sử dụng SSL hay không
        /// </summary>
        public bool UseSsl { get; init; } = true;
    }
}