namespace KPISolution.Models.ViewModels.Indicator.PerformanceIndicator
{
    public class PerformanceIndicatorCreateViewModel
    {
        [Required(ErrorMessage = "Yêu cầu nhập Mã chỉ số.")]
        [StringLength(10, ErrorMessage = "Mã chỉ số không được vượt quá 10 ký tự.")]
        [Display(Name = "Mã chỉ số")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập Tên chỉ số.")]
        [StringLength(100, ErrorMessage = "Tên chỉ số không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên chỉ số")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập Mô tả.")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập Công thức tính.")]
        [StringLength(200, ErrorMessage = "Công thức tính không được vượt quá 200 ký tự.")]
        [Display(Name = "Công thức tính")]
        public string Formula { get; set; } = string.Empty;

        [Display(Name = "Đơn vị đo")]
        public MeasurementUnit Unit { get; set; }

        [Display(Name = "Tần suất đo")]
        public MeasurementFrequency Frequency { get; set; }

        [Display(Name = "Tần suất đánh giá")]
        public ReviewFrequency ReviewFrequency { get; set; }

        [Display(Name = "Loại hoạt động")]
        public ActivityType ActivityType { get; set; }

        [Display(Name = "Mức hiệu suất")]
        public PerformanceLevel PerformanceLevel { get; set; }

        [Display(Name = "Mức kiểm soát")]
        public ControlLevel ControlLevel { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập Kế hoạch hành động.")]
        [StringLength(500, ErrorMessage = "Kế hoạch hành động không được vượt quá 500 ký tự.")]
        [Display(Name = "Kế hoạch hành động")]
        public string ActionPlan { get; set; } = string.Empty;

        [Display(Name = "Phương pháp thu thập dữ liệu")]
        public DataCollectionMethod DataCollectionMethod { get; set; }

        [Display(Name = "Yếu tố thành công")]
        public Guid? ResultIndicatorId { get; set; }

        [Display(Name = "Người phụ trách")]
        public string? ResponsibleTeamMemberId { get; set; }
    }
}
