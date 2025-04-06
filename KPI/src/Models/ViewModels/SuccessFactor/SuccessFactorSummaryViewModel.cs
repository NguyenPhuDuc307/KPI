namespace KPISolution.Models.ViewModels.SuccessFactor
{
    /// <summary>
    /// Hiển thị thông tin tóm tắt về Critical Success Factor để hiển thị trong danh sách hoặc dashboard
    /// </summary>
    public class SuccessFactorSummaryViewModel
    {
        /// <summary>
        /// ID của Success Factor
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tên của Success Factor
        /// </summary>
        [Display(Name = "Tên")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mã code của Success Factor
        /// </summary>
        [Display(Name = "Mã")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Phần trăm tiến độ hoàn thành
        /// </summary>
        [Display(Name = "Tiến độ")]
        [Range(0, 100)]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// CSS class để hiển thị tiến độ
        /// </summary>
        public string ProgressCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái của Success Factor
        /// </summary>
        [Display(Name = "Trạng thái")]
        public SuccessFactorStatus Status { get; set; }

        /// <summary>
        /// CSS class để hiển thị trạng thái
        /// </summary>
        public string StatusCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Văn bản hiển thị trạng thái
        /// </summary>
        [Display(Name = "Trạng thái")]
        public string StatusDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Ngày cập nhật gần nhất
        /// </summary>
        [Display(Name = "Cập nhật lần cuối")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Ngày hoàn thành dự kiến
        /// </summary>
        [Display(Name = "Ngày hoàn thành")]
        public DateTime? TargetDate { get; set; }

        /// <summary>
        /// Số ngày còn lại để hoàn thành
        /// </summary>
        [Display(Name = "Số ngày còn lại")]
        public int DaysRemaining { get; set; }

        /// <summary>
        /// Phòng ban phụ trách
        /// </summary>
        [Display(Name = "Phòng ban")]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Mục tiêu liên quan
        /// </summary>
        [Display(Name = "Mục tiêu")]
        public string Objective { get; set; } = string.Empty;

        /// <summary>
        /// Người phụ trách
        /// </summary>
        [Display(Name = "Người phụ trách")]
        public string Owner { get; set; } = string.Empty;
    }
}