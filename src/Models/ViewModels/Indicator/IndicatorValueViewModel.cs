namespace KPISolution.Models.ViewModels.Indicator
{
    /// <summary>
    /// Hiển thị giá trị của một indicator trong danh sách đo lường
    /// </summary>
    public class IndicatorValueViewModel
    {
        /// <summary>
        /// ID của phép đo
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID của chỉ số (Tên mới, nên dùng cái này)
        /// </summary>
        public Guid IndicatorId { get; set; }

        /// <summary>
        /// Mã chỉ số
        /// </summary>
        [Display(Name = "Mã chỉ số")]
        public string IndicatorCode { get; set; } = string.Empty;

        /// <summary>
        /// Tên chỉ số
        /// </summary>
        [Display(Name = "Tên chỉ số")]
        public string IndicatorName { get; set; } = string.Empty;

        /// <summary>
        /// Loại chỉ số
        /// </summary>
        [Display(Name = "Loại chỉ số")]
        public string IndicatorType { get; set; } = string.Empty;

        /// <summary>
        /// Phòng ban
        /// </summary>
        [Display(Name = "Phòng ban")]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị đo được (Đổi tên từ Value)
        /// </summary>
        [Display(Name = "Giá trị đo được")]
        public decimal ActualValue { get; set; }

        /// <summary>
        /// Giá trị mục tiêu
        /// </summary>
        [Display(Name = "Giá trị mục tiêu")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Đơn vị đo lường
        /// </summary>
        [Display(Name = "Đơn vị")]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Kỳ đo lường (ví dụ: Tháng 3/2023)
        /// </summary>
        [Display(Name = "Kỳ")]
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// Phần trăm đạt được so với mục tiêu
        /// </summary>
        [Display(Name = "% đạt được")]
        public decimal AchievementPercentage { get; set; }

        /// <summary>
        /// Sai lệch so với mục tiêu
        /// </summary>
        [Display(Name = "Sai lệch")]
        public decimal Variance { get; set; }

        /// <summary>
        /// Trạng thái của chỉ số
        /// </summary>
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// CSS class để hiển thị trạng thái
        /// </summary>
        public string StatusCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Ngày thực hiện đo lường
        /// </summary>
        [Display(Name = "Ngày đo")]
        [DataType(DataType.Date)]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// Người thực hiện đo lường
        /// </summary>
        [Display(Name = "Người đo")]
        public string MeasuredBy { get; set; } = string.Empty;

        /// <summary>
        /// Người tạo phép đo (Thêm mới)
        /// </summary>
        [Display(Name = "Người tạo")]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Ghi chú
        /// </summary>
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }
    }
}
