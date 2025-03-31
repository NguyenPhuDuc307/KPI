using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.CSF
{
    /// <summary>
    /// View model for editing an existing Critical Success Factor
    /// </summary>
    public class EditCsfViewModel
    {
        /// <summary>
        /// Constructor to initialize collections and SelectList properties
        /// </summary>
        public EditCsfViewModel()
        {
            SelectedKpiIds = new List<Guid>();
            LinkedKpis = new List<CsfKpiRelationshipViewModel>();
            RecentProgressUpdates = new List<CsfProgressSummaryViewModel>();
            BusinessObjectives = new SelectList(Array.Empty<SelectListItem>());
            Departments = new SelectList(Array.Empty<SelectListItem>());
            AvailableKpis = new SelectList(Array.Empty<SelectListItem>());
        }

        /// <summary>
        /// Unique identifier for the CSF
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the Critical Success Factor
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "CSF Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Critical Success Factor
        /// </summary>
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Unique code for identifying this CSF
        /// </summary>
        [Required(ErrorMessage = "Code is required")]
        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Code must contain only uppercase letters, numbers, and hyphens")]
        [Display(Name = "CSF Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// The strategic objective this CSF is related to
        /// </summary>
        [StringLength(200, ErrorMessage = "Strategic objective cannot exceed 200 characters")]
        [Display(Name = "Strategic Objective")]
        public string? StrategicObjective { get; set; }

        /// <summary>
        /// Reference to the business objective this CSF supports
        /// </summary>
        [Display(Name = "Business Objective")]
        public Guid? BusinessObjectiveId { get; set; }

        /// <summary>
        /// Priority of this CSF
        /// </summary>
        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        /// <summary>
        /// Status of this CSF
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public CSFStatus Status { get; set; } = CSFStatus.NotStarted;

        /// <summary>
        /// Category of this CSF
        /// </summary>
        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public CSFCategory Category { get; set; } = CSFCategory.Other;

        /// <summary>
        /// Start date when this CSF becomes active
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Target date for achieving this CSF
        /// </summary>
        [Required(ErrorMessage = "Target date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; } = DateTime.Today.AddYears(1);

        /// <summary>
        /// Department responsible for this CSF
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Person who owns or is responsible for this CSF
        /// </summary>
        [StringLength(100, ErrorMessage = "Owner name cannot exceed 100 characters")]
        [Display(Name = "Owner")]
        public string? Owner { get; set; }

        /// <summary>
        /// Risk level associated with this CSF
        /// </summary>
        [Required(ErrorMessage = "Risk level is required")]
        [Display(Name = "Risk Level")]
        public RiskLevel RiskLevel { get; set; } = RiskLevel.Medium;

        /// <summary>
        /// Additional notes or comments about this CSF
        /// </summary>
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Current progress percentage
        /// </summary>
        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100")]
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; } = 0;

        /// <summary>
        /// List of selected KPI IDs to link to this CSF
        /// </summary>
        [Display(Name = "Linked KPIs")]
        public List<Guid> SelectedKpiIds { get; set; } = new List<Guid>();

        /// <summary>
        /// Currently linked KPIs with relationship information
        /// </summary>
        public List<CsfKpiRelationshipViewModel> LinkedKpis { get; set; } = new List<CsfKpiRelationshipViewModel>();

        /// <summary>
        /// Last review date of this CSF's progress
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
        /// Dropdown items for available KPIs
        /// </summary>
        public SelectList AvailableKpis { get; set; }

        /// <summary>
        /// Collection of recent progress updates for this CSF
        /// </summary>
        public List<CsfProgressSummaryViewModel> RecentProgressUpdates { get; set; } = new List<CsfProgressSummaryViewModel>();

        /// <summary>
        /// Record concurrency token
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; set; }

        /// <summary>
        /// Success criteria for this CSF
        /// </summary>
        [Display(Name = "Tiêu chí thành công")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string SuccessCriteria { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the relationship between a CSF and a KPI in a view-friendly format
    /// </summary>
    public class CsfKpiRelationshipViewModel
    {
        /// <summary>
        /// The KPI ID
        /// </summary>
        public Guid KpiId { get; set; }

        /// <summary>
        /// The KPI name
        /// </summary>
        public string KpiName { get; set; } = string.Empty;

        /// <summary>
        /// The KPI code
        /// </summary>
        public string KpiCode { get; set; } = string.Empty;

        /// <summary>
        /// Type of KPI (KRI, RI, PI)
        /// </summary>
        public KpiType KpiType { get; set; }

        /// <summary>
        /// The strength of relationship between this CSF and KPI
        /// </summary>
        [Required(ErrorMessage = "Relationship strength is required")]
        public RelationshipStrength RelationshipStrength { get; set; } = RelationshipStrength.Strong;

        /// <summary>
        /// Description of how this KPI contributes to the CSF
        /// </summary>
        [StringLength(200)]
        public string? ContributionDescription { get; set; }

        /// <summary>
        /// The weight or importance of this KPI to the CSF
        /// </summary>
        [Range(0, 100, ErrorMessage = "Weight must be between 0 and 100")]
        public int Weight { get; set; } = 0;

        /// <summary>
        /// The impact level this KPI has on the CSF
        /// </summary>
        [Required(ErrorMessage = "Impact level is required")]
        public ImpactLevel ImpactLevel { get; set; } = ImpactLevel.High;
    }

    /// <summary>
    /// Summary view model for CSF progress updates
    /// </summary>
    public class CsfProgressSummaryViewModel
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
        /// Status of the CSF at this update point
        /// </summary>
        [Display(Name = "Status")]
        public CSFStatus Status { get; set; }

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
        /// A brief summary of the update combining achievements, challenges, and next steps
        /// </summary>
        [Display(Name = "Summary")]
        public string? Summary { get; set; }
    }
}
