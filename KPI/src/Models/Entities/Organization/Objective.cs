using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Organization
{
    /// <summary>
    /// Represents a strategic or business objective within the organization
    /// </summary>
    public class Objective : BaseEntity
    {
        /// <summary>
        /// Unique code identifier for the objective (format: OBJ-YYYYMM###)
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Objective Code")]
        public string Code { get; init; } = string.Empty;

        /// <summary>
        /// Name or title of the objective
        /// </summary>
        [Required]
        [StringLength(200)]
        [Display(Name = "Objective Name")]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Detailed description of the objective
        /// </summary>
        [Required]
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Business perspective this objective belongs to
        /// </summary>
        [Required]
        [Display(Name = "Business Perspective")]
        public BusinessPerspective BusinessPerspective { get; init; }

        /// <summary>
        /// Priority level of this objective
        /// </summary>
        [Required]
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; init; }

        /// <summary>
        /// Current status of this objective
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public ObjectiveStatus Status { get; init; } = ObjectiveStatus.NotStarted;

        /// <summary>
        /// Progress percentage of this objective (0-100)
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; init; } = 0;

        /// <summary>
        /// Start date of this objective
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; init; }

        /// <summary>
        /// Target completion date of this objective
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; init; }

        /// <summary>
        /// Actual completion date of this objective
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Completion Date")]
        public DateTime? CompletionDate { get; init; }

        /// <summary>
        /// Department responsible for this objective
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; init; }

        /// <summary>
        /// Navigation property to department
        /// </summary>
        [ForeignKey("DepartmentId")]
        public Department? Department { get; init; }

        /// <summary>
        /// ID of the person responsible for this objective
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Responsible Person")]
        public string? ResponsiblePersonId { get; init; }

        /// <summary>
        /// Parent objective ID, if this is a sub-objective
        /// </summary>
        [Display(Name = "Parent Objective")]
        public Guid? ParentId { get; init; }

        /// <summary>
        /// Navigation property to parent objective
        /// </summary>
        [ForeignKey("ParentId")]
        public Objective? Parent { get; init; }

        /// <summary>
        /// Collection of child objectives
        /// </summary>
        public virtual ICollection<Objective>? Children { get; init; }

        /// <summary>
        /// Collection of Success Factors related to this objective
        /// </summary>
        public virtual ICollection<SuccessFactor>? SuccessFactors { get; init; }

        /// <summary>
        /// Budget allocated for this objective
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Budget")]
        public decimal? Budget { get; init; }

        /// <summary>
        /// Additional notes or comments about this objective
        /// </summary>
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? Notes { get; init; }

        /// <summary>
        /// Fiscal year this objective belongs to
        /// </summary>
        [StringLength(9)]
        [Display(Name = "Fiscal Year")]
        public string? FiscalYear { get; init; }

        /// <summary>
        /// Objective's timeframe (Short-term, Medium-term, Long-term)
        /// </summary>
        [Display(Name = "Timeframe")]
        public TimeframeType Timeframe { get; init; }
    }
}
