using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for adding a measurement to a KPI
    /// </summary>
    public class AddKpiMeasurementViewModel
    {
        /// <summary>
        /// ID of the KPI to add measurement for
        /// </summary>
        [Required]
        public Guid KpiId { get; set; }

        /// <summary>
        /// The actual value recorded for the KPI
        /// </summary>
        [Required]
        [Display(Name = "Value")]
        public decimal Value { get; set; }

        /// <summary>
        /// Date and time when the measurement was taken
        /// </summary>
        [Required]
        [Display(Name = "Measurement Date")]
        [DataType(DataType.Date)]
        public DateTime MeasurementDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The period this measurement belongs to (e.g., Jan 2023, Q1 2023)
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Period")]
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// The type of period (daily, weekly, monthly, quarterly, yearly)
        /// </summary>
        [Required]
        [Display(Name = "Period Type")]
        public PeriodType PeriodType { get; set; } = PeriodType.Monthly;

        /// <summary>
        /// Status of the measurement
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public MeasurementStatus Status { get; set; } = MeasurementStatus.Recorded;

        /// <summary>
        /// Additional notes or comments about the measurement
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Source of the measurement data
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Data Source")]
        public string? DataSource { get; set; }

        /// <summary>
        /// KPI name (for display purposes)
        /// </summary>
        [Display(Name = "KPI Name")]
        public string KpiName { get; set; } = string.Empty;

        /// <summary>
        /// KPI code (for display purposes)
        /// </summary>
        [Display(Name = "KPI Code")]
        public string KpiCode { get; set; } = string.Empty;

        /// <summary>
        /// Unit of measurement (for display purposes)
        /// </summary>
        [Display(Name = "Unit")]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Target value (for display purposes)
        /// </summary>
        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }
    }
}