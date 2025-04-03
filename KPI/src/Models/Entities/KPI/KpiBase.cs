using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Enums;

namespace KPISolution.Models.Entities.KPI
{
    /// <summary>
    /// Base class for all KPI entities
    /// </summary>
    public abstract class KpiBase : BaseEntity
    {
        /// <summary>
        /// Name of the KPI
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "KPI Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the KPI
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// KPI code used for reference
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "KPI Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Measurement unit for the KPI (e.g., percentage, days, count)
        /// </summary>
        [Required]
        [StringLength(30)]
        [Display(Name = "Unit")]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Formula used to calculate the KPI value
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Calculation Formula")]
        public string? Formula { get; set; }

        /// <summary>
        /// Target or goal value for the KPI
        /// </summary>
        [Display(Name = "Target Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Current value of the KPI
        /// </summary>
        [Display(Name = "Current Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? CurrentValue { get; set; }

        /// <summary>
        /// Minimum acceptable value for the KPI
        /// </summary>
        [Display(Name = "Minimum Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal MinimumValue { get; set; }

        /// <summary>
        /// Maximum possible value for the KPI
        /// </summary>
        [Display(Name = "Maximum Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal MaximumValue { get; set; }

        /// <summary>
        /// Weight or importance of this KPI (often used for calculations)
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Weight (%)")]
        public int Weight { get; set; }

        /// <summary>
        /// Frequency of measurement (daily, weekly, monthly, quarterly, annually)
        /// </summary>
        [Required]
        [Display(Name = "Frequency")]
        public MeasurementFrequency Frequency { get; set; } = MeasurementFrequency.Monthly;

        /// <summary>
        /// Department or division responsible for this KPI
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Responsible Department")]
        public string? Department { get; set; }

        /// <summary>
        /// Person responsible for this KPI
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Responsible Person")]
        public string? ResponsiblePerson { get; set; }

        /// <summary>
        /// Date the KPI becomes effective
        /// </summary>
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Current status of the KPI (active, draft, archived)
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public KpiStatus Status { get; set; } = KpiStatus.Active;

        /// <summary>
        /// Date and time when the KPI was last modified
        /// </summary>
        [Display(Name = "Modified At")]
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// ID of the user who last modified the KPI
        /// </summary>
        [Display(Name = "Modified By")]
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Measurement direction indicating whether higher or lower values are better
        /// </summary>
        [Required]
        [Display(Name = "Measurement Direction")]
        public MeasurementDirection MeasurementDirection { get; set; } = MeasurementDirection.HigherIsBetter;

        /// <summary>
        /// Historical performance trend
        /// </summary>
        [Display(Name = "Performance Trend")]
        public PerformanceTrend? PerformanceTrend { get; set; }

        /// <summary>
        /// Collection of measurement values
        /// </summary>
        public virtual ICollection<KpiValue> Values { get; set; } = new List<KpiValue>();
    }
}
