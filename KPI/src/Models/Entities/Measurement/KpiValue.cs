using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Entities.KPI;

namespace KPISolution.Models.Entities.Measurement
{
    /// <summary>
    /// Represents a specific measurement/value of a KPI at a point in time
    /// </summary>
    public class KpiValue : BaseEntity
    {
        /// <summary>
        /// Reference to the KPI this value belongs to
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
        /// The actual measured value
        /// </summary>
        [Required]
        [Display(Name = "Actual Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal ActualValue { get; set; }

        /// <summary>
        /// The target value for comparison
        /// </summary>
        [Display(Name = "Target Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// The date and time of measurement
        /// </summary>
        [Required]
        [Display(Name = "Measurement Date")]
        [DataType(DataType.DateTime)]
        public DateTime MeasurementDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The period this measurement belongs to (e.g., Jan 2023, Q1 2023)
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Period")]
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// Comments or notes about this measurement
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// The source of this data point
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Data Source")]
        public string? DataSource { get; set; }

        /// <summary>
        /// The status of this measurement (On Target, Below Target, Above Target)
        /// </summary>
        [StringLength(20)]
        [Display(Name = "Status")]
        public string? Status { get; set; }

        /// <summary>
        /// Percentage of target achieved
        /// </summary>
        [Display(Name = "Achievement (%)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AchievementPercentage { get; set; }

        /// <summary>
        /// Trend compared to previous measurement (Improving, Stable, Declining)
        /// </summary>
        [StringLength(20)]
        [Display(Name = "Trend")]
        public string? Trend { get; set; }

        /// <summary>
        /// Variance from target
        /// </summary>
        [Display(Name = "Variance")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? Variance { get; set; }

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
