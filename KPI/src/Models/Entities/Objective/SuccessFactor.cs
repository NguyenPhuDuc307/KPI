using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Enums;

namespace KPISolution.Models.Entities.Objective
{
    /// <summary>
    /// Represents a Success Factor (SF) - factors that contribute to achieving an objective
    /// </summary>
    public class SuccessFactor : BaseEntity
    {
        /// <summary>
        /// Name of the Success Factor
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "SF Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Success Factor
        /// </summary>
        [Required]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Unique code for identifying this SF
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "SF Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Priority of this SF (1-5, where 5 is highest)
        /// </summary>
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        /// <summary>
        /// Current status of this SF
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public ObjectiveStatus Status { get; set; } = ObjectiveStatus.NotStarted;

        /// <summary>
        /// Indicates whether this Success Factor is a Critical Success Factor (CSF)
        /// </summary>
        [Display(Name = "Is Critical Success Factor")]
        public bool IsCSF { get; set; } = false;

        /// <summary>
        /// The business objective this SF supports
        /// </summary>
        [Required]
        [Display(Name = "Business Objective")]
        public Guid BusinessObjectiveId { get; set; }

        /// <summary>
        /// Navigation property to the business objective
        /// </summary>
        [ForeignKey("BusinessObjectiveId")]
        public virtual BusinessObjective BusinessObjective { get; set; } = null!;

        /// <summary>
        /// Current progress percentage of this SF
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; } = 0;

        /// <summary>
        /// Start date when this SF becomes active
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Target date for achieving this SF
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; } = DateTime.UtcNow.AddYears(1);

        /// <summary>
        /// Department responsible for this SF
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Navigation property to the responsible department
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Person who owns or is responsible for this SF
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Owner")]
        public string? Owner { get; set; }

        /// <summary>
        /// Additional notes or comments about this SF
        /// </summary>
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Collection of Critical Success Factors (CSFs) that belong to this SF
        /// </summary>
        public virtual ICollection<CriticalSuccessFactor>? CriticalSuccessFactors { get; set; }
    }
}