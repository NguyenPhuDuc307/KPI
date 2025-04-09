using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Indicator
{
    /// <summary>
    /// Base class for Indicator entities that provides common properties and behaviors
    /// </summary>
    public abstract class IndicatorBase : BaseEntity
    {
        /// <summary>
        /// Name of the Indicator
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Indicator
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Unique code for identifying this Indicator
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Priority level of this Indicator
        /// </summary>
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        /// <summary>
        /// Department responsible for this Indicator
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Navigation property to the responsible department
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Person who owns or is responsible for this Indicator
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Owner")]
        public string? Owner { get; set; }

        /// <summary>
        /// User responsible for this Indicator
        /// </summary>
        [Display(Name = "Responsible User")]
        public string? ResponsibleUserId { get; set; }

        /// <summary>
        /// Navigation property to the responsible user
        /// </summary>
        [ForeignKey("ResponsibleUserId")]
        public virtual ApplicationUser? ResponsibleUser { get; set; }

        /// <summary>
        /// Current status of this Indicator
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public IndicatorStatus Status { get; set; } = IndicatorStatus.Draft;

        /// <summary>
        /// Additional notes or comments about this Indicator
        /// </summary>
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Start date when this Indicator becomes active
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Target date for achieving this Indicator
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime? TargetDate { get; set; }

        /// <summary>
        /// Last review date of this Indicator
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
