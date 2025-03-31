using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Enums;

namespace KPISolution.Models.Entities.CSF
{
    /// <summary>
    /// Represents a Critical Success Factor (CSF) - elements that are vital for a business strategy to succeed
    /// </summary>
    public class CriticalSuccessFactor : BaseEntity
    {
        /// <summary>
        /// Name of the Critical Success Factor
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "CSF Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Critical Success Factor
        /// </summary>
        [Required]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Unique code for identifying this CSF
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "CSF Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// The strategic objective this CSF is related to
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Strategic Objective")]
        public string? StrategicObjective { get; set; }

        /// <summary>
        /// Reference to the business objective this CSF supports
        /// </summary>
        [Display(Name = "Business Objective")]
        public Guid? BusinessObjectiveId { get; set; }

        /// <summary>
        /// Navigation property to the business objective
        /// </summary>
        [ForeignKey("BusinessObjectiveId")]
        public virtual BusinessObjective? BusinessObjective { get; set; }

        /// <summary>
        /// Priority of this CSF (1-5, where 5 is highest)
        /// </summary>
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        /// <summary>
        /// Current progress percentage of this CSF
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; } = 0;

        /// <summary>
        /// Current status of this CSF
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public CSFStatus Status { get; set; } = CSFStatus.NotStarted;

        /// <summary>
        /// Category of this CSF
        /// </summary>
        [Required]
        [Display(Name = "Category")]
        public CSFCategory Category { get; set; } = CSFCategory.Other;

        /// <summary>
        /// Start date when this CSF becomes active
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Target date for achieving this CSF
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; } = DateTime.UtcNow.AddYears(1);

        /// <summary>
        /// Department responsible for this CSF
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Navigation property to the responsible department
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Person who owns or is responsible for this CSF
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Owner")]
        public string? Owner { get; set; }

        /// <summary>
        /// Risk level associated with this CSF
        /// </summary>
        [Display(Name = "Risk Level")]
        public RiskLevel RiskLevel { get; set; } = RiskLevel.Medium;

        /// <summary>
        /// Additional notes or comments about this CSF
        /// </summary>
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Collection of KPI-CSF relationship mappings
        /// </summary>
        public virtual ICollection<CSFKPI>? CSFKPIs { get; set; }

        /// <summary>
        /// Collection of progress updates for this CSF
        /// </summary>
        public virtual ICollection<CSFProgress>? ProgressUpdates { get; set; }

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
    }
}
