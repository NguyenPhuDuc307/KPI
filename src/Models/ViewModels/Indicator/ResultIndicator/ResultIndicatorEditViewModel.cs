namespace KPISolution.Models.ViewModels.Indicator.ResultIndicator
{
    /// <summary>
    /// View model for creating and editing Result Indicators (RI and KRI)
    /// </summary>
    public class ResultIndicatorEditViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Tên chỉ số là bắt buộc")]
        [Display(Name = "Tên chỉ số")]
        [StringLength(100, ErrorMessage = "Tên chỉ số không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Display(Name = "Mã chỉ số")]
        [StringLength(20, ErrorMessage = "Mã chỉ số không được vượt quá 20 ký tự")]
        public string? Code { get; set; }

        [Display(Name = "Chỉ số kết quả chính")]
        public bool IsKey { get; set; }

        [Display(Name = "Công thức")]
        [StringLength(200, ErrorMessage = "Công thức không được vượt quá 200 ký tự")]
        public string? Formula { get; set; }

        [Display(Name = "Giá trị mục tiêu")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị mục tiêu phải lớn hơn 0")]
        public decimal? TargetValue { get; set; }

        [Required]
        [Display(Name = "Đơn vị")]
        public MeasurementUnit Unit { get; set; }
        public IEnumerable<SelectListItem> UnitOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Cách diễn giải giá trị")]
        public MeasurementDirection Direction { get; set; } = MeasurementDirection.HigherIsBetter;

        [Required]
        [Display(Name = "Tần suất đo lường")]
        public MeasurementFrequency Frequency { get; set; }
        public IEnumerable<SelectListItem> FrequencyOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Phạm vi đo lường")]
        public MeasurementScope? MeasurementScope { get; set; }
        public IEnumerable<SelectListItem> MeasurementScopeOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Lĩnh vực quy trình")]
        public ProcessArea? ProcessArea { get; set; }
        public IEnumerable<SelectListItem> ProcessAreaOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Khung thời gian")]
        public TimeFrame? TimeFrame { get; set; }
        public IEnumerable<SelectListItem> TimeFrameOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Nguồn dữ liệu")]
        public DataSource? DataSource { get; set; }
        public IEnumerable<SelectListItem> DataSourceOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Loại kết quả")]
        public ResultType? ResultType { get; set; }
        public IEnumerable<SelectListItem> ResultTypeOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Đóng góp (%)")]
        [Range(0, 100, ErrorMessage = "Phần trăm đóng góp phải nằm trong khoảng từ 0 đến 100")]
        public int? ContributionPercentage { get; set; }

        // Relationships
        [Required]
        [Display(Name = "SuccessFactor")]
        public Guid SuccessFactorId { get; set; }
        public IEnumerable<SelectListItem> SuccessFactorOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Người quản lý phụ trách")]
        public Guid? ResponsibleManagerId { get; set; }
        public IEnumerable<SelectListItem> ResponsibleManagerOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Người phụ trách")]
        public Guid? ResponsibleUserId { get; set; }
        public IEnumerable<SelectListItem> ResponsibleUserOptions { get; set; } = new List<SelectListItem>();

        // UI helper properties
        public string PageTitle => this.Id.HasValue ? "Chỉnh sửa " + (this.IsKey ? "Chỉ số kết quả chính" : "Chỉ số kết quả") : "Tạo mới " + (this.IsKey ? "Chỉ số kết quả chính" : "Chỉ số kết quả");
        public string SubmitButtonText => this.Id.HasValue ? "Cập nhật" : "Tạo mới";
        public string CancelUrl => "/ResultIndicator";

        // Helper method to determine which fields should be shown based on type
        public bool ShowKriSpecificFields => this.IsKey;
    }
}
