namespace KPISolution.Models.ViewModels.Indicator
{
    /// <summary>
    /// Hiển thị thông tin tóm tắt về một Indicator để hiển thị trong danh sách hoặc dashboard
    /// </summary>
    public class IndicatorSummaryViewModel
    {
        /// <summary>
        /// ID của Indicator
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tên của Indicator
        /// </summary>
        [Display(Name = "Tên")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mã code của Indicator
        /// </summary>
        [Display(Name = "Mã")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị mục tiêu của Indicator
        /// </summary>
        [Display(Name = "Mục tiêu")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Giá trị hiện tại của Indicator
        /// </summary>
        [Display(Name = "Giá trị hiện tại")]
        public decimal? CurrentValue { get; set; }

        /// <summary>
        /// Đơn vị đo lường
        /// </summary>
        [Display(Name = "Đơn vị")]
        public string MeasurementUnit { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái của Indicator
        /// </summary>
        [Display(Name = "Trạng thái")]
        public IndicatorStatus Status { get; set; }

        /// <summary>
        /// Phòng ban phụ trách Indicator
        /// </summary>
        [Display(Name = "Phòng ban")]
        public string Department { get; set; } = string.Empty;

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
        /// Ngày đo lường gần nhất
        /// </summary>
        [Display(Name = "Cập nhật lần cuối")]
        public DateTime? LastMeasurementDate { get; set; }

        /// <summary>
        /// Phần trăm đạt được so với mục tiêu
        /// </summary>
        [Display(Name = "% Hoàn thành")]
        public decimal? PercentageComplete { get; set; }

        /// <summary>
        /// Hướng xu hướng của Indicator
        /// </summary>
        [Display(Name = "Xu hướng")]
        public TrendDirection Trend { get; set; }

        /// <summary>
        /// Văn bản hiển thị xu hướng
        /// </summary>
        [Display(Name = "Xu hướng")]
        public string TrendDisplay { get; set; } = string.Empty;
    }
}