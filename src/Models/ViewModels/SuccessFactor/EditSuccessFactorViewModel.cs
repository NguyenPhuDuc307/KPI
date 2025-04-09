using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Models.ViewModels.SuccessFactor
{
    /// <summary>
    /// View model for editing an existing Success Factor
    /// </summary>
    public class EditSuccessFactorViewModel
    {
        /// <summary>
        /// Constructor to initialize collections and SelectList properties
        /// </summary>
        public EditSuccessFactorViewModel()
        {
            this.SelectedPerformanceIndicatorIds = [];
            this.LinkedPerformanceIndicators = [];
            this.RecentProgressUpdates = [];
            this.BusinessObjectives = new SelectList(Array.Empty<SelectListItem>());
            this.Departments = new SelectList(Array.Empty<SelectListItem>());
            this.AvailablePerformanceIndicators = new SelectList(Array.Empty<SelectListItem>());
            this.AvailableSuccessFactors = new SelectList(Array.Empty<SelectListItem>());
        }

        /// <summary>
        /// Unique identifier for the Success Factor
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the Success Factor
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        [Display(Name = "Success Factor Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Success Factor
        /// </summary>
        [Display(Name = "Mô tả chi tiết")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        /// <summary>
        /// Unique code for identifying this Success Factor
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập mã yếu tố")]
        [StringLength(20, ErrorMessage = "Mã yếu tố không được vượt quá 20 ký tự")]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Mã yếu tố chỉ chấp nhận chữ in hoa, số và ký tự - hoặc _")]
        [Display(Name = "Mã yếu tố")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// The strategic objective this Success Factor is related to
        /// </summary>
        [StringLength(200, ErrorMessage = "Strategic objective cannot exceed 200 characters")]
        [Display(Name = "Strategic Objective")]
        public string? StrategicObjective { get; set; }

        /// <summary>
        /// Reference to the business objective this Success Factor supports
        /// </summary>
        [Display(Name = "Business Objective")]
        public Guid? BusinessObjectiveId { get; set; }

        /// <summary>
        /// Priority of this Success Factor
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn mức độ ưu tiên")]
        [Display(Name = "Mức độ ưu tiên")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        /// <summary>
        /// Status of this Success Factor
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Trạng thái hiện tại")]
        public SuccessFactorStatus Status { get; set; } = SuccessFactorStatus.NotStarted;

        /// <summary>
        /// Category of this Success Factor
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public SuccessFactorCategory Category { get; set; } = SuccessFactorCategory.Other;

        /// <summary>
        /// Indicates whether this is a Critical Success Factor
        /// </summary>
        [Display(Name = "Critical Success Factor")]
        public bool IsCritical { get; set; } = false;

        /// <summary>
        /// Parent Success Factor that this one belongs to
        /// </summary>
        [Display(Name = "Parent Success Factor")]
        public Guid? ParentSuccessFactorId { get; set; }

        /// <summary>
        /// Start date when this Success Factor becomes active
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Target date for achieving this Success Factor
        /// </summary>
        [Required(ErrorMessage = "Target date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; } = DateTime.Today.AddYears(1);

        /// <summary>
        /// Department responsible for this Success Factor
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn phòng ban chịu trách nhiệm")]
        [Display(Name = "Phòng ban chịu trách nhiệm")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Person who owns or is responsible for this Success Factor
        /// </summary>
        [StringLength(100, ErrorMessage = "Owner name cannot exceed 100 characters")]
        [Display(Name = "Owner")]
        public string? Owner { get; set; }

        /// <summary>
        /// Risk level associated with this Success Factor
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn mức độ rủi ro")]
        [Display(Name = "Mức độ rủi ro")]
        public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;

        /// <summary>
        /// Additional notes or comments about this Success Factor
        /// </summary>
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Current progress percentage
        /// </summary>
        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100")]
        [Display(Name = "% Hoàn thành hiện tại")]
        public int ProgressPercentage { get; set; } = 0;

        /// <summary>
        /// List of selected Performance Indicator IDs to link to this Success Factor
        /// </summary>
        [Display(Name = "Linked Performance Indicators")]
        public List<Guid> SelectedPerformanceIndicatorIds { get; set; } = [];

        /// <summary>
        /// Currently linked Performance Indicators with relationship information
        /// </summary>
        public List<SuccessFactorIndicatorRelationshipViewModel> LinkedPerformanceIndicators { get; set; } = [];

        /// <summary>
        /// Last review date of this Success Factor's progress
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Last Review Date")]
        public DateTime? LastReviewDate { get; set; }

        /// <summary>
        /// Next scheduled review date
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Next Review Date")]
        public DateTime? NextReviewDate { get; set; }

        /// <summary>
        /// Dropdown items for business objectives
        /// </summary>
        public SelectList BusinessObjectives { get; set; }

        /// <summary>
        /// Dropdown items for departments
        /// </summary>
        public SelectList Departments { get; set; }

        /// <summary>
        /// Dropdown items for available Performance Indicators
        /// </summary>
        public SelectList AvailablePerformanceIndicators { get; set; }

        /// <summary>
        /// Dropdown items for available parent Success Factors
        /// </summary>
        public SelectList AvailableSuccessFactors { get; set; }

        /// <summary>
        /// Collection of recent progress updates for this Success Factor
        /// </summary>
        public List<SuccessFactorProgressSummaryViewModel> RecentProgressUpdates { get; set; } = [];

        /// <summary>
        /// Record concurrency token
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; set; }

        /// <summary>
        /// Success criteria for this Success Factor
        /// </summary>
        [Display(Name = "Success Criteria")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string SuccessCriteria { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the relationship between a Success Factor and a Performance Indicator in a view-friendly format
    /// </summary>
    public class SuccessFactorIndicatorRelationshipViewModel
    {
        /// <summary>
        /// The Performance Indicator ID
        /// </summary>
        public Guid PerformanceIndicatorId { get; set; }

        /// <summary>
        /// The Performance Indicator name
        /// </summary>
        public string PerformanceIndicatorName { get; set; } = string.Empty;

        /// <summary>
        /// The Performance Indicator code
        /// </summary>
        public string PerformanceIndicatorCode { get; set; } = string.Empty;

        /// <summary>
        /// Type of Performance Indicator (RI, PI)
        /// </summary>
        public IndicatorType IndicatorType { get; set; }

        /// <summary>
        /// The strength of the relationship between the Success Factor and Performance Indicator
        /// </summary>
        [Required(ErrorMessage = "Relationship strength is required")]
        [Display(Name = "Relationship Strength")]
        public RelationshipStrength RelationshipStrength { get; set; } = RelationshipStrength.Medium;

        /// <summary>
        /// Description of how this Performance Indicator contributes to the Success Factor
        /// </summary>
        [StringLength(200)]
        public string? ContributionDescription { get; set; }

        /// <summary>
        /// The weight or importance of this Performance Indicator to the Success Factor
        /// </summary>
        [Range(0, 100, ErrorMessage = "Weight must be between 0 and 100")]
        public int Weight { get; set; } = 0;

        /// <summary>
        /// The impact level this Performance Indicator has on the Success Factor
        /// </summary>
        [Required(ErrorMessage = "Impact level is required")]
        public ImpactLevel ImpactLevel { get; set; } = ImpactLevel.Medium;
    }

    /// <summary>
    /// Summary view model for Success Factor progress updates
    /// </summary>
    public class SuccessFactorProgressSummaryViewModel
    {
        /// <summary>
        /// Progress update ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Date of the progress update
        /// </summary>
        [Display(Name = "Update Date")]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// The progress percentage at this update point
        /// </summary>
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Status of the Success Factor at this update point
        /// </summary>
        [Display(Name = "Status")]
        public SuccessFactorStatus Status { get; set; }

        /// <summary>
        /// Risk level at this update point
        /// </summary>
        [Display(Name = "Risk Level")]
        public RiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Person who provided this update
        /// </summary>
        [Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Flag indicating if this update requires management attention
        /// </summary>
        [Display(Name = "Needs Attention")]
        public bool NeedsAttention { get; set; }

        /// <summary>
        /// Brief summary of the update
        /// </summary>
        [Display(Name = "Summary")]
        public string? Summary { get; set; }
    }
}
