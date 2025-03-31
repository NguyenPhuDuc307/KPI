using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Enums;

namespace KPISolution.Models.Entities.Organization
{
    /// <summary>
    /// Represents a business objective within the organization
    /// </summary>
    public class BusinessObjective : BaseEntity
    {
        /// <summary>
        /// Name or title of the business objective
        /// </summary>
        [Required]
        [StringLength(200)]
        [Display(Name = "Objective Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the business objective
        /// </summary>
        [Required]
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Business perspective this objective belongs to
        /// </summary>
        [Required]
        [Display(Name = "Business Perspective")]
        public BusinessPerspective BusinessPerspective { get; set; }

        /// <summary>
        /// Priority level of this objective
        /// </summary>
        [Required]
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; }

        /// <summary>
        /// Current status of this objective
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public ObjectiveStatus Status { get; set; } = ObjectiveStatus.NotStarted;

        /// <summary>
        /// Progress percentage of this objective (0-100)
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; } = 0;

        /// <summary>
        /// Start date of this objective
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Target completion date of this objective
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Actual completion date of this objective
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Completion Date")]
        public DateTime? CompletionDate { get; set; }

        /// <summary>
        /// Department responsible for this objective
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Navigation property to department
        /// </summary>
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        /// <summary>
        /// ID of the person responsible for this objective
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Responsible Person")]
        public string? ResponsiblePersonId { get; set; }

        /// <summary>
        /// Parent objective ID, if this is a sub-objective
        /// </summary>
        [Display(Name = "Parent Objective")]
        public Guid? ParentObjectiveId { get; set; }

        /// <summary>
        /// Navigation property to parent objective
        /// </summary>
        [ForeignKey("ParentObjectiveId")]
        public BusinessObjective? ParentObjective { get; set; }

        /// <summary>
        /// Collection of child objectives
        /// </summary>
        public virtual ICollection<BusinessObjective>? ChildObjectives { get; set; }

        /// <summary>
        /// Budget allocated for this objective
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Budget")]
        public decimal? Budget { get; set; }

        /// <summary>
        /// Additional notes or comments about this objective
        /// </summary>
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Fiscal year this objective belongs to
        /// </summary>
        [StringLength(9)]
        [Display(Name = "Fiscal Year")]
        public string? FiscalYear { get; set; }

        /// <summary>
        /// Objective's timeframe (Short-term, Medium-term, Long-term)
        /// </summary>
        [Display(Name = "Timeframe")]
        public TimeframeType Timeframe { get; set; }
    }
}
