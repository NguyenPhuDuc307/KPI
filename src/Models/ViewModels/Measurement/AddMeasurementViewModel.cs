namespace KPISolution.Models.ViewModels.Measurement
{
    public class AddMeasurementViewModel
    {
        public Guid IndicatorId { get; set; }

        [Display(Name = "Mã chỉ số")]
        public string IndicatorCode { get; set; } = string.Empty;

        [Display(Name = "Tên chỉ số")]
        public string IndicatorName { get; set; } = string.Empty;

        [Display(Name = "Loại chỉ số")]
        public string IndicatorType { get; set; } = string.Empty;

        [Display(Name = "Đơn vị")]
        public string? Unit { get; set; }

        [Display(Name = "Giá trị mục tiêu")]
        public string? TargetValue { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá trị thực tế")]
        [Display(Name = "Giá trị thực tế")]
        public decimal? ActualValue { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày đo lường")]
        [Display(Name = "Ngày đo lường")]
        [DataType(DataType.Date)]
        public DateTime MeasurementDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Loại kỳ đo (Thêm mới)
        /// </summary>
        [Display(Name = "Loại kỳ đo")]
        public PeriodType? PeriodType { get; set; }

        /// <summary>
        /// Nguồn dữ liệu (Thêm mới)
        /// </summary>
        [Display(Name = "Nguồn dữ liệu")]
        public string? DataSource { get; set; }

        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        [Display(Name = "Trạng thái")]
        public MeasurementStatus Status { get; set; } = MeasurementStatus.Actual;

        public string? ReturnUrl { get; set; }
    }
}
