namespace KPISolution.Models.ViewModels.Measurement
{
    /// <summary>
    /// ViewModel cho form tạo mới phép đo
    /// </summary>
    public class MeasurementCreateViewModel
    {
        /// <summary>
        /// ID chung của Indicator (Thêm mới để khớp với View Create)
        /// </summary>
        public Guid IndicatorId { get; set; }

        /// <summary>
        /// ID của chỉ số loại Performance Indicator (nếu có)
        /// </summary>
        public Guid? PerformanceIndicatorId { get; set; }

        /// <summary>
        /// ID của chỉ số loại Result Indicator (nếu có)
        /// </summary>
        public Guid? ResultIndicatorId { get; set; }

        /// <summary>
        /// ID của chỉ số loại Success Factor (nếu có)
        /// </summary>
        public Guid? SuccessFactorId { get; set; }

        /// <summary>
        /// Tên chỉ số
        /// </summary>
        [Display(Name = "Chỉ số")]
        public string IndicatorName { get; set; } = string.Empty;

        /// <summary>
        /// Mã chỉ số
        /// </summary>
        [Display(Name = "Mã chỉ số")]
        public string IndicatorCode { get; set; } = string.Empty;

        /// <summary>
        /// Loại phép đo
        /// </summary>
        [Display(Name = "Loại chỉ số")]
        public IndicatorMeasurementType Type { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        [Display(Name = "Đơn vị")]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị đo được (Đổi tên từ Value)
        /// </summary>
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá trị đo được")]
        public decimal ActualValue { get; set; }

        /// <summary>
        /// Giá trị mục tiêu
        /// </summary>
        [Display(Name = "Giá trị mục tiêu")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Ngưỡng cảnh báo tối thiểu
        /// </summary>
        [Display(Name = "Ngưỡng cảnh báo tối thiểu")]
        public decimal? MinAlertThreshold { get; set; }

        /// <summary>
        /// Ngưỡng cảnh báo tối đa
        /// </summary>
        [Display(Name = "Ngưỡng cảnh báo tối đa")]
        public decimal? MaxAlertThreshold { get; set; }

        /// <summary>
        /// Ngày thực hiện đo lường
        /// </summary>
        [Required(ErrorMessage = "Ngày đo lường không được để trống")]
        [Display(Name = "Ngày đo lường")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// Tần suất đo lường
        /// </summary>
        [Display(Name = "Tần suất đo lường")]
        public MeasurementFrequency Frequency { get; set; }

        /// <summary>
        /// Trạng thái phép đo
        /// </summary>
        [Display(Name = "Trạng thái")]
        public MeasurementStatus Status { get; set; } = MeasurementStatus.Actual;

        /// <summary>
        /// Ghi chú về phép đo
        /// </summary>
        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }

        // UI helper properties
        public string PageTitle => "Thêm phép đo mới cho " + this.IndicatorName;
        public string SubmitButtonText => "Lưu";
        public string? CancelUrl { get; set; }
    }
}
