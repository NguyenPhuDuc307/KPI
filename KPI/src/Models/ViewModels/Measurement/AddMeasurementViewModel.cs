namespace KPISolution.Models.ViewModels.Measurement
{
    public class AddMeasurementViewModel
    {
        public Guid IndicatorId { get; init; }

        [Display(Name = "Mã chỉ số")]
        public string IndicatorCode { get; init; } = string.Empty;

        [Display(Name = "Tên chỉ số")]
        public string IndicatorName { get; init; } = string.Empty;

        [Display(Name = "Loại chỉ số")]
        public string IndicatorType { get; init; } = string.Empty;

        [Display(Name = "Đơn vị")]
        public string? Unit { get; init; }

        [Display(Name = "Giá trị mục tiêu")]
        public string? TargetValue { get; init; }

        [Required(ErrorMessage = "Vui lòng nhập giá trị thực tế")]
        [Display(Name = "Giá trị thực tế")]
        public decimal? ActualValue { get; init; }

        [Required(ErrorMessage = "Vui lòng chọn ngày đo lường")]
        [Display(Name = "Ngày đo lường")]
        [DataType(DataType.Date)]
        public DateTime MeasurementDate { get; init; } = DateTime.Today;

        /// <summary>
        /// Loại kỳ đo (Thêm mới)
        /// </summary>
        [Display(Name = "Loại kỳ đo")]
        public PeriodType? PeriodType { get; init; }

        /// <summary>
        /// Nguồn dữ liệu (Thêm mới)
        /// </summary>
        [Display(Name = "Nguồn dữ liệu")]
        public string? DataSource { get; init; }

        [Display(Name = "Ghi chú")]
        public string? Notes { get; init; }

        [Display(Name = "Trạng thái")]
        public MeasurementStatus Status { get; init; } = MeasurementStatus.Actual;

        public string? ReturnUrl { get; init; }
    }
}
