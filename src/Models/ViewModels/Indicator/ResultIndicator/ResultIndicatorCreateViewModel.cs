namespace KPISolution.Models.ViewModels.Indicator.ResultIndicator
{
    /// <summary>
    /// ViewModel for creating a new Result Indicator
    /// </summary>
    public class ResultIndicatorCreateViewModel
    {
        /// <summary>
        /// Name of the Result Indicator
        /// </summary>
        [Required(ErrorMessage = "Tên chỉ số là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên chỉ số không được vượt quá 100 ký tự")]
        [Display(Name = "Tên chỉ số")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Code of the Result Indicator
        /// </summary>
        [Required(ErrorMessage = "Mã chỉ số là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã chỉ số không được vượt quá 20 ký tự")]
        [Display(Name = "Mã chỉ số")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Result Indicator
        /// </summary>
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        /// <summary>
        /// Flag indicating if this is a Key Result Indicator (KRI)
        /// </summary>
        [Display(Name = "Chỉ số kết quả chính (KRI)")]
        public bool IsKey { get; set; }

        /// <summary>
        /// Target value of the Result Indicator
        /// </summary>
        [Display(Name = "Giá trị mục tiêu")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Threshold value of the Result Indicator
        /// </summary>
        [Display(Name = "Ngưỡng giá trị")]
        public decimal? ThresholdValue { get; set; }

        /// <summary>
        /// Priority level of the Result Indicator
        /// </summary>
        [Display(Name = "Mức độ ưu tiên")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        /// <summary>
        /// Related Success Factor ID
        /// </summary>
        [Display(Name = "SuccessFactor")]
        public Guid? SuccessFactorId { get; set; }

        /// <summary>
        /// Department ID responsible for this Result Indicator
        /// </summary>
        [Display(Name = "Phòng ban")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        [Display(Name = "Đơn vị")]
        public MeasurementUnit Unit { get; set; } = MeasurementUnit.Number;

        /// <summary>
        /// Frequency of measurement
        /// </summary>
        [Display(Name = "Tần suất đo lường")]
        public MeasurementFrequency Frequency { get; set; } = MeasurementFrequency.Monthly;

        /// <summary>
        /// Direction of measurement (whether higher or lower values are better)
        /// </summary>
        [Display(Name = "Cách diễn giải giá trị")]
        public MeasurementDirection Direction { get; set; } = MeasurementDirection.HigherIsBetter;

        /// <summary>
        /// Notes about this Result Indicator
        /// </summary>
        [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        /// <summary>
        /// User responsible for this Result Indicator
        /// </summary>
        [Display(Name = "Người phụ trách")]
        public Guid? ResponsibleUserId { get; set; }
        public IEnumerable<SelectListItem> ResponsibleUserOptions { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Start date of the Result Indicator
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Target date for achieving this Result Indicator
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime TargetDate { get; set; } = DateTime.Today.AddYears(1);
    }
}
