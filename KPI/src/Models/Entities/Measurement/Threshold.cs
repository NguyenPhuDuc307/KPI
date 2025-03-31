using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Entities.KPI;

namespace KPISolution.Models.Entities.Measurement
{
    /// <summary>
    /// Represents threshold values for a KPI that determine its status
    /// </summary>
    public class Threshold : BaseEntity
    {
        /// <summary>
        /// Reference to the KPI this threshold belongs to
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
        /// Minimum value for "Red" status (critical/poor performance)
        /// </summary>
        [Display(Name = "Red Threshold")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal RedThreshold { get; set; }

        /// <summary>
        /// Minimum value for "Yellow" status (warning/mediocre performance)
        /// </summary>
        [Display(Name = "Yellow Threshold")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal YellowThreshold { get; set; }

        /// <summary>
        /// Minimum value for "Green" status (good/acceptable performance)
        /// </summary>
        [Display(Name = "Green Threshold")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal GreenThreshold { get; set; }

        /// <summary>
        /// Indicates if higher values are better for this KPI
        /// </summary>
        [Display(Name = "Higher is Better")]
        public bool HigherIsBetter { get; set; } = true;

        /// <summary>
        /// Description of what red status means for this KPI
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Red Description")]
        public string? RedDescription { get; set; }

        /// <summary>
        /// Description of what yellow status means for this KPI
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Yellow Description")]
        public string? YellowDescription { get; set; }

        /// <summary>
        /// Description of what green status means for this KPI
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Green Description")]
        public string? GreenDescription { get; set; }

        /// <summary>
        /// Effective date for these thresholds
        /// </summary>
        [Required]
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Expiration date for these thresholds
        /// </summary>
        [Display(Name = "Expiration Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

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
