using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Enums;

namespace KPISolution.Models.Entities.KPI
{
    /// <summary>
    /// Performance Indicator (PI) - Measures what must be done to improve performance in a specific area
    /// </summary>
    public class PI : KpiBase
    {
        /// <summary>
        /// Reference to the parent RI
        /// </summary>
        [Display(Name = "Parent RI")]
        public Guid? RIId { get; set; }

        /// <summary>
        /// Navigation property to the parent RI
        /// </summary>
        [ForeignKey("RIId")]
        public virtual RI? ParentRI { get; set; }

        /// <summary>
        /// Reference to the parent KRI (direct relationship without going through RI)
        /// </summary>
        [Display(Name = "Parent KRI")]
        public Guid? KRIId { get; set; }

        /// <summary>
        /// Navigation property to the parent KRI
        /// </summary>
        [ForeignKey("KRIId")]
        public virtual KRI? ParentKRI { get; set; }

        /// <summary>
        /// Reference to the CSF this PI is associated with
        /// </summary>
        [Display(Name = "Critical Success Factor")]
        public Guid? CriticalSuccessFactorId { get; set; }

        /// <summary>
        /// Navigation property to the associated CSF
        /// </summary>
        [ForeignKey("CriticalSuccessFactorId")]
        public virtual CriticalSuccessFactor? CriticalSuccessFactor { get; set; }

        /// <summary>
        /// Indicates whether this is a Key Performance Indicator (KPI). If true, this Performance Indicator
        /// is considered a KPI for the organization.
        /// </summary>
        [Display(Name = "Is Key Performance Indicator (KPI)")]
        public bool IsKey { get; set; } = false;

        /// <summary>
        /// Activity type categorization for this Performance Indicator
        /// </summary>
        [Required]
        [Display(Name = "Activity Type")]
        public ActivityType ActivityType { get; set; } = ActivityType.StandardOperations;

        /// <summary>
        /// Performance level rating
        /// </summary>
        [Display(Name = "Performance Level")]
        [Range(1, 5)]
        public int PerformanceLevel { get; set; } = 3;

        /// <summary>
        /// Level of control over this indicator (e.g., High, Medium, Low)
        /// </summary>
        [StringLength(20)]
        [Display(Name = "Control Level")]
        public string? ControlLevel { get; set; } = "High";

        /// <summary>
        /// Whether this indicator is leading or lagging
        /// </summary>
        [Required]
        [Display(Name = "Indicator Type")]
        public IndicatorType IndicatorType { get; set; } = IndicatorType.Leading;

        /// <summary>
        /// The specific actions to be taken when the indicator shows poor performance
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Action Plan")]
        public string? ActionPlan { get; set; }

        /// <summary>
        /// The data collection method for this PI
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Data Collection Method")]
        public string? DataCollectionMethod { get; set; }

        /// <summary>
        /// The contribution percentage this PI has toward its parent RI
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Contribution to RI (%)")]
        public int? ContributionPercentage { get; set; }

        /// <summary>
        /// The team member responsible for this performance indicator
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Team Member")]
        public string? ResponsibleTeamMember { get; set; }

        /// <summary>
        /// Frequency of review for this indicator
        /// </summary>
        [StringLength(20)]
        [Display(Name = "Review Frequency")]
        public string? ReviewFrequency { get; set; } = "Weekly";

        /// <summary>
        /// Threshold value that triggers an alert or action
        /// </summary>
        [Display(Name = "Alert Threshold")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? AlertThreshold { get; set; }
    }
}
