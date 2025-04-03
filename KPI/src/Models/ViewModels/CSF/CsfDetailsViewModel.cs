using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.CSF
{
    /// <summary>
    /// View model for displaying detailed information about a Critical Success Factor
    /// </summary>
    public class CsfDetailsViewModel
    {
        /// <summary>
        /// Unique identifier for the CSF
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the Critical Success Factor
        /// </summary>
        [Display(Name = "CSF Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Critical Success Factor
        /// </summary>
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Unique code for identifying this CSF
        /// </summary>
        [Display(Name = "CSF Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// The strategic objective this CSF is related to
        /// </summary>
        [Display(Name = "Strategic Objective")]
        public string? StrategicObjective { get; set; }

        /// <summary>
        /// Reference to the business objective this CSF supports
        /// </summary>
        [Display(Name = "Business Objective ID")]
        public Guid? BusinessObjectiveId { get; set; }

        /// <summary>
        /// Name of the business objective
        /// </summary>
        [Display(Name = "Business Objective")]
        public string? BusinessObjectiveName { get; set; }

        /// <summary>
        /// Priority of this CSF
        /// </summary>
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; }

        /// <summary>
        /// Priority as a display-friendly string
        /// </summary>
        [Display(Name = "Priority")]
        public string PriorityDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Status of this CSF
        /// </summary>
        [Display(Name = "Status")]
        public CSFStatus Status { get; set; }

        /// <summary>
        /// Status as a display-friendly string
        /// </summary>
        [Display(Name = "Status")]
        public string StatusDisplay { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for status styling
        /// </summary>
        public string StatusCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Category of this CSF
        /// </summary>
        [Display(Name = "Category")]
        public CSFCategory Category { get; set; }

        /// <summary>
        /// Category as a display-friendly string
        /// </summary>
        [Display(Name = "Category")]
        public string CategoryDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Start date when this CSF becomes active
        /// </summary>
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Target date for achieving this CSF
        /// </summary>
        [Display(Name = "Target Date")]
        [DataType(DataType.Date)]
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Department responsible for this CSF
        /// </summary>
        [Display(Name = "Department ID")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Name of the responsible department
        /// </summary>
        [Display(Name = "Department")]
        public string? DepartmentName { get; set; }

        /// <summary>
        /// Person who owns or is responsible for this CSF
        /// </summary>
        [Display(Name = "Owner")]
        public string? Owner { get; set; }

        /// <summary>
        /// Risk level associated with this CSF
        /// </summary>
        [Display(Name = "Risk Level")]
        public RiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Risk level as a display-friendly string
        /// </summary>
        [Display(Name = "Risk Level")]
        public string RiskLevelDisplay { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for risk level styling
        /// </summary>
        public string RiskLevelCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Additional notes or comments about this CSF
        /// </summary>
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Current progress percentage
        /// </summary>
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// CSS class for progress styling based on percentage
        /// </summary>
        public string ProgressCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Collection of KPIs linked to this CSF
        /// </summary>
        [Display(Name = "Linked KPIs")]
        public List<LinkedKpiViewModel> LinkedKpis { get; set; } = new List<LinkedKpiViewModel>();

        /// <summary>
        /// Collection of progress updates for this CSF
        /// </summary>
        [Display(Name = "Progress History")]
        public List<CsfProgressHistoryViewModel> ProgressUpdates { get; set; } = new List<CsfProgressHistoryViewModel>();

        /// <summary>
        /// Collection of update history for this CSF
        /// </summary>
        [Display(Name = "Update History")]
        public List<CsfUpdateHistoryViewModel> UpdateHistory { get; set; } = new List<CsfUpdateHistoryViewModel>();

        /// <summary>
        /// Last review date of this CSF's progress
        /// </summary>
        [Display(Name = "Last Review Date")]
        [DataType(DataType.Date)]
        public DateTime? LastReviewDate { get; set; }

        /// <summary>
        /// Next scheduled review date
        /// </summary>
        [Display(Name = "Next Review Date")]
        [DataType(DataType.Date)]
        public DateTime? NextReviewDate { get; set; }

        /// <summary>
        /// Days remaining until the target date
        /// </summary>
        [Display(Name = "Days Remaining")]
        public int DaysRemaining { get; set; }

        /// <summary>
        /// Percentage of time elapsed
        /// </summary>
        [Display(Name = "Time Elapsed (%)")]
        public int TimeElapsedPercentage { get; set; }

        /// <summary>
        /// Flag indicating if progress is on track compared to elapsed time
        /// </summary>
        [Display(Name = "On Track")]
        public bool IsOnTrack { get; set; }

        /// <summary>
        /// Creation date of the CSF
        /// </summary>
        [Display(Name = "Created On")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// User who created the CSF
        /// </summary>
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Last modification date
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// User who last modified the CSF
        /// </summary>
        [Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }
    }

    /// <summary>
    /// View model for linked KPIs
    /// </summary>
    public class LinkedKpiViewModel
    {
        /// <summary>
        /// KPI ID
        /// </summary>
        public Guid KpiId { get; set; }

        /// <summary>
        /// KPI Name
        /// </summary>
        [Display(Name = "KPI Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// KPI Code
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// KPI Type
        /// </summary>
        [Display(Name = "Type")]
        public KpiType KpiType { get; set; }

        /// <summary>
        /// KPI Status
        /// </summary>
        [Display(Name = "Status")]
        public KpiStatus Status { get; set; }

        /// <summary>
        /// KPI type as a display-friendly string
        /// </summary>
        [Display(Name = "Type")]
        public string KpiTypeDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Current value of the KPI
        /// </summary>
        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }

        /// <summary>
        /// Target value for the KPI
        /// </summary>
        [Display(Name = "Target")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Measurement unit of the KPI
        /// </summary>
        [Display(Name = "Unit")]
        public string? Unit { get; set; }

        /// <summary>
        /// The relationship strength between the CSF and this KPI
        /// </summary>
        [Display(Name = "Relationship Strength")]
        public RelationshipStrength RelationshipStrength { get; set; }

        /// <summary>
        /// Relationship strength as a display-friendly string
        /// </summary>
        [Display(Name = "Relationship")]
        public string RelationshipStrengthDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Description of how this KPI contributes to the CSF
        /// </summary>
        [Display(Name = "Contribution")]
        public string? ContributionDescription { get; set; }

        /// <summary>
        /// The weight of this KPI to the CSF
        /// </summary>
        [Display(Name = "Weight (%)")]
        public int Weight { get; set; }

        /// <summary>
        /// The impact level this KPI has on the CSF
        /// </summary>
        [Display(Name = "Impact")]
        public ImpactLevel ImpactLevel { get; set; }

        /// <summary>
        /// Impact level as a display-friendly string
        /// </summary>
        [Display(Name = "Impact")]
        public string ImpactLevelDisplay { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for styling based on KPI performance
        /// </summary>
        public string PerformanceCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Flag indicating if this is sample data
        /// </summary>
        [Display(Name = "Is Sample Data")]
        public bool IsSampleData { get; set; } = false;
    }

    /// <summary>
    /// View model for CSF progress updates history
    /// </summary>
    public class CsfProgressHistoryViewModel
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
        /// Previous progress percentage
        /// </summary>
        [Display(Name = "Previous (%)")]
        public int? PreviousProgressPercentage { get; set; }

        /// <summary>
        /// Change in progress since last update
        /// </summary>
        [Display(Name = "Change (%)")]
        public int? ProgressChange { get; set; }

        /// <summary>
        /// Status of the CSF at this update point
        /// </summary>
        [Display(Name = "Status")]
        public CSFStatus Status { get; set; }

        /// <summary>
        /// Status as a display-friendly string
        /// </summary>
        [Display(Name = "Status")]
        public string StatusDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Previous status
        /// </summary>
        [Display(Name = "Previous Status")]
        public CSFStatus? PreviousStatus { get; set; }

        /// <summary>
        /// Previous status as a display-friendly string
        /// </summary>
        [Display(Name = "Previous Status")]
        public string? PreviousStatusDisplay { get; set; }

        /// <summary>
        /// Risk level at this update point
        /// </summary>
        [Display(Name = "Risk Level")]
        public RiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Risk level as a display-friendly string
        /// </summary>
        [Display(Name = "Risk")]
        public string RiskLevelDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Previous risk level
        /// </summary>
        [Display(Name = "Previous Risk")]
        public RiskLevel? PreviousRiskLevel { get; set; }

        /// <summary>
        /// Commentary on what has been achieved
        /// </summary>
        [Display(Name = "Achievements")]
        public string? Achievements { get; set; }

        /// <summary>
        /// Commentary on challenges or blockers
        /// </summary>
        [Display(Name = "Challenges")]
        public string? Challenges { get; set; }

        /// <summary>
        /// Next steps planned
        /// </summary>
        [Display(Name = "Next Steps")]
        public string? NextSteps { get; set; }

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
        /// CSS class for styling based on progress change
        /// </summary>
        public string ProgressChangeCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Expected completion date based on current progress
        /// </summary>
        [Display(Name = "Expected Completion")]
        [DataType(DataType.Date)]
        public DateTime? ExpectedCompletionDate { get; set; }
    }

    /// <summary>
    /// View model for CSF update history
    /// </summary>
    public class CsfUpdateHistoryViewModel
    {
        /// <summary>
        /// Update ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Update date and time
        /// </summary>
        [Display(Name = "Update Date")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// The user who made the update
        /// </summary>
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Notes about the update
        /// </summary>
        [Display(Name = "Notes")]
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Progress change (positive or negative)
        /// </summary>
        [Display(Name = "Progress Change")]
        public int? ProgressChange { get; set; }

        /// <summary>
        /// Status change information
        /// </summary>
        [Display(Name = "Status Change")]
        public string? StatusChange { get; set; }
    }
}
