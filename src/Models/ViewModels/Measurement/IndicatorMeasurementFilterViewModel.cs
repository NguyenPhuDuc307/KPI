namespace KPISolution.Models.ViewModels.Measurement
{
    /// <summary>
    /// ViewModel cho bộ lọc đo lường
    /// </summary>
    public class IndicatorMeasurementFilterViewModel
    {
        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        [Display(Name = "Tìm kiếm")]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Phòng ban
        /// </summary>
        [Display(Name = "Phòng ban")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Loại chỉ số (dùng cho bộ lọc)
        /// </summary>
        [Display(Name = "Loại chỉ số")]
        public IndicatorType? IndicatorType { get; set; }

        /// <summary>
        /// Tần suất đo lường
        /// </summary>
        [Display(Name = "Tần suất")]
        public MeasurementFrequency? Frequency { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [Display(Name = "Trạng thái")]
        public MeasurementStatus? Status { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Ngày bắt đầu (dùng cho tương thích API mới)
        /// </summary>
        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Ngày kết thúc (dùng cho tương thích API mới)
        /// </summary>
        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }
}
