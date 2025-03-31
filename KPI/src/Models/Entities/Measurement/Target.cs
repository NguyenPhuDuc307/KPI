using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Entities.KPI;

namespace KPISolution.Models.Entities.Measurement
{
    /// <summary>
    /// Represents a target or goal for a KPI for a specific period
    /// </summary>
    public class Target : BaseEntity
    {
        /// <summary>
        /// Reference to the KPI this target belongs to
        /// </summary>
        [Required]
        [Display(Name = "KPI ID")]
        public Guid KpiId { get; set; }

        /// <summary>
        /// Type of KPI (KRI, RI, PI)
        /// </summary>
        [Required]
        [StringLength(10)]
        [Display(Name = "KPI Type")]
        public string KpiType { get; set; } = string.Empty;

        /// <summary>
        /// The target value to achieve
        /// </summary>
        [Required]
        [Display(Name = "Target Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal TargetValue { get; set; }

        /// <summary>
        /// The period this target is for (e.g., Jan 2023, Q1 2023, FY 2023)
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Period")]
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// Start date of the target period
        /// </summary>
        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// End date of the target period
        /// </summary>
        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);

        /// <summary>
        /// Description or rationale for this target
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Type of target (Stretch, Committed, Minimum)
        /// </summary>
        [StringLength(20)]
        [Display(Name = "Target Type")]
        public string? TargetType { get; set; } = "Committed";

        /// <summary>
        /// Person who approved this target
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Approved By")]
        public string? ApprovedBy { get; set; }

        /// <summary>
        /// Date when this target was approved
        /// </summary>
        [Display(Name = "Approval Date")]
        [DataType(DataType.Date)]
        public DateTime? ApprovalDate { get; set; }

        /// <summary>
        /// The stretch target (beyond normal target)
        /// </summary>
        [Display(Name = "Stretch Target")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? StretchTarget { get; set; }

        /// <summary>
        /// The minimum acceptable target
        /// </summary>
        [Display(Name = "Minimum Target")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? MinimumTarget { get; set; }

        /// <summary>
        /// Current status of this target (Pending, Active, Achieved, Missed)
        /// </summary>
        [StringLength(20)]
        [Display(Name = "Status")]
        public string? Status { get; set; } = "Pending";

        /// <summary>
        /// Navigation property to KRI if KpiType is KRI
        /// </summary>
        [ForeignKey("KpiId")]
        public virtual KRI? KRI { get; set; }

        /// <summary>
        /// Navigation property to RI if KpiType is RI
        /// </summary>
        [ForeignKey("KpiId")]
        public virtual RI? RI { get; set; }

        /// <summary>
        /// Navigation property to PI if KpiType is PI
        /// </summary>
        [ForeignKey("KpiId")]
        public virtual PI? PI { get; set; }
    }
}
