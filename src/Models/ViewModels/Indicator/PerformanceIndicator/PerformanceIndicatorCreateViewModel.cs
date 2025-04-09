namespace KPISolution.Models.ViewModels.Indicator.PerformanceIndicator
{
    public class PerformanceIndicatorCreateViewModel
    {
        [Required(ErrorMessage = "Yêu cầu nhập Mã chỉ số.")]
        [StringLength(20, ErrorMessage = "Mã chỉ số không được vượt quá 20 ký tự.")]
        [Display(Name = "Mã chỉ số")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập Tên chỉ số.")]
        [StringLength(100, ErrorMessage = "Tên chỉ số không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên chỉ số")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Công thức tính không được vượt quá 200 ký tự.")]
        [Display(Name = "Công thức tính")]
        public string? Formula { get; set; }

        [Display(Name = "Đơn vị đo")]
        public MeasurementUnit Unit { get; set; }

        [Display(Name = "Giá trị mục tiêu")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị mục tiêu phải lớn hơn 0")]
        public decimal? TargetValue { get; set; }

        [Display(Name = "Ngưỡng cảnh báo tối thiểu")]
        [Range(0, double.MaxValue, ErrorMessage = "Ngưỡng cảnh báo tối thiểu phải lớn hơn 0")]
        public decimal? MinAlertThreshold { get; set; }

        [Display(Name = "Ngưỡng cảnh báo tối đa")]
        [Range(0, double.MaxValue, ErrorMessage = "Ngưỡng cảnh báo tối đa phải lớn hơn 0")]
        public decimal? MaxAlertThreshold { get; set; }

        [Display(Name = "Tần suất đo")]
        public MeasurementFrequency Frequency { get; set; }

        [Display(Name = "Tần suất đánh giá")]
        public ReviewFrequency ReviewFrequency { get; set; }

        [Display(Name = "Loại hoạt động")]
        public ActivityType ActivityType { get; set; }

        [Display(Name = "Control Level")]
        public ControlLevel? ControlLevel { get; set; }
        public IEnumerable<SelectListItem> ControlLevelOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Action Plan")]
        [StringLength(500, ErrorMessage = "Action plan cannot exceed 500 characters")]
        public string? ActionPlan { get; set; }

        [Display(Name = "Phương pháp thu thập dữ liệu")]
        public DataCollectionMethod DataCollectionMethod { get; set; }

        [Display(Name = "Result Indicator")]
        public Guid? ResultIndicatorId { get; set; }

        [Display(Name = "Success Factor")]
        public Guid? SuccessFactorId { get; set; }

        [Display(Name = "Người phụ trách")]
        public Guid? ResponsibleTeamMemberId { get; set; }
        public IEnumerable<SelectListItem> ResponsibleTeamMemberOptions { get; set; } = new List<SelectListItem>();
    }
}
