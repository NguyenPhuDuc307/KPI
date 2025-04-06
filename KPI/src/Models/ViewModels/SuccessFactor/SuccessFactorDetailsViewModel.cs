namespace KPISolution.Models.ViewModels.SuccessFactor
{
    /// <summary>
    /// View model for displaying Success Factor details.
    /// </summary>
    public class SuccessFactorDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the Success Factor ID.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Gets or sets the unique code.
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the Success Factor name.
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the Success Factor description.
        /// </summary>
        [Display(Name = "Description")]
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets whether this is a Critical Success Factor.
        /// </summary>
        [Display(Name = "Critical")]
        public bool IsCritical { get; init; }

        /// <summary>
        /// Gets or sets the Priority of this Success Factor.
        /// </summary>
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; init; }

        /// <summary>
        /// Gets or sets the Risk Level.
        /// </summary>
        [Display(Name = "Risk Level")]
        public RiskLevel RiskLevel { get; init; }

        /// <summary>
        /// Gets or sets the category of this Success Factor.
        /// </summary>
        [Display(Name = "Category")]
        public SuccessFactorCategory Category { get; init; }

        /// <summary>
        /// Gets or sets the progress percentage.
        /// </summary>
        [Display(Name = "Progress")]
        [Range(0, 100)]
        public int ProgressPercentage { get; init; }

        /// <summary>
        /// Gets or sets the status of the Success Factor.
        /// </summary>
        [Display(Name = "Status")]
        public SuccessFactorStatus Status { get; init; }

        /// <summary>
        /// Gets or sets the parent Success Factor ID.
        /// </summary>
        public Guid? ParentSuccessFactorId { get; init; }

        /// <summary>
        /// Gets or sets the parent Success Factor name.
        /// </summary>
        [Display(Name = "Parent Success Factor")]
        public string ParentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the objective ID.
        /// </summary>
        public Guid? ObjectiveId { get; init; }

        /// <summary>
        /// Gets or sets the objective name.
        /// </summary>
        [Display(Name = "Objective")]
        public string ObjectiveName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        public Guid? DepartmentId { get; init; }

        /// <summary>
        /// Gets or sets the department name.
        /// </summary>
        [Display(Name = "Department")]
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; init; }

        /// <summary>
        /// Gets or sets the last updated date.
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; init; }

        /// <summary>
        /// Gets or sets the number of indicators linked to this Success Factor
        /// </summary>
        [Display(Name = "Indicator Count")]
        public int IndicatorCount { get; init; }

        /// <summary>
        /// Gets or sets the collection of indicators associated with this success factor.
        /// </summary>
        public List<IndicatorViewModel> Indicators { get; init; } = [];

        /// <summary>
        /// Lịch sử cập nhật tiến độ
        /// </summary>
        public List<ProgressUpdateViewModel> ProgressHistory { get; set; } = [];
    }

    /// <summary>
    /// Thông tin cập nhật tiến độ
    /// </summary>
    public class ProgressUpdateViewModel
    {
        /// <summary>
        /// ID của bản ghi cập nhật
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Phần trăm tiến độ
        /// </summary>
        [Display(Name = "Tiến độ")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Phần trăm tiến độ trước đó
        /// </summary>
        [Display(Name = "Tiến độ trước")]
        public int PreviousPercentage { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [Display(Name = "Trạng thái")]
        public SuccessFactorStatus Status { get; set; }

        /// <summary>
        /// Trạng thái trước đó
        /// </summary>
        [Display(Name = "Trạng thái trước")]
        public SuccessFactorStatus PreviousStatus { get; set; }

        /// <summary>
        /// Mức độ rủi ro
        /// </summary>
        [Display(Name = "Mức độ rủi ro")]
        public RiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Mức độ rủi ro trước đó
        /// </summary>
        [Display(Name = "Mức độ rủi ro trước")]
        public RiskLevel PreviousRiskLevel { get; set; }

        /// <summary>
        /// Nhận xét
        /// </summary>
        [Display(Name = "Nhận xét")]
        public string? Comments { get; set; }

        /// <summary>
        /// Vấn đề gặp phải
        /// </summary>
        [Display(Name = "Vấn đề")]
        public string? Issues { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [Display(Name = "Người cập nhật")]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        [Display(Name = "Thời gian")]
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// View model đơn giản cho các chỉ số
    /// </summary>
    public class IndicatorViewModel
    {
        /// <summary>
        /// ID của chỉ số
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tên chỉ số
        /// </summary>
        [Display(Name = "Tên")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mã chỉ số
        /// </summary>
        [Display(Name = "Mã")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái chỉ số
        /// </summary>
        [Display(Name = "Trạng thái")]
        public IndicatorStatus Status { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        [Display(Name = "Đơn vị")]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị hiện tại
        /// </summary>
        [Display(Name = "Giá trị hiện tại")]
        public decimal CurrentValue { get; set; }
    }
}
