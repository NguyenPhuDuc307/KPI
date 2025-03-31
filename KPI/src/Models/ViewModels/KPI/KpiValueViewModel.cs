using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Entities.Measurement;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for displaying KPI measurement values
    /// </summary>
    public class KpiValueViewModel
    {
        /// <summary>
        /// Unique identifier for the value
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Reference to the KPI this value belongs to
        /// </summary>
        [Display(Name = "KPI ID")]
        public Guid KpiId { get; set; }

        /// <summary>
        /// Type of KPI (KRI, RI, PI)
        /// </summary>
        [Display(Name = "KPI Type")]
        public string KpiType { get; set; } = string.Empty;

        /// <summary>
        /// The actual measured value
        /// </summary>
        [Display(Name = "Actual Value")]
        public decimal ActualValue { get; set; }

        /// <summary>
        /// The target value for comparison
        /// </summary>
        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// The date and time of measurement
        /// </summary>
        [Display(Name = "Measurement Date")]
        [DataType(DataType.DateTime)]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// The period this measurement belongs to (e.g., Jan 2023, Q1 2023)
        /// </summary>
        [Display(Name = "Period")]
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// Comments or notes about this measurement
        /// </summary>
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// The source of this data point
        /// </summary>
        [Display(Name = "Data Source")]
        public string? DataSource { get; set; }

        /// <summary>
        /// The status of this measurement (On Target, Below Target, Above Target)
        /// </summary>
        [Display(Name = "Status")]
        public string? Status { get; set; }

        /// <summary>
        /// Percentage of target achieved
        /// </summary>
        [Display(Name = "Achievement (%)")]
        public decimal? AchievementPercentage { get; set; }

        /// <summary>
        /// Trend compared to previous measurement (Improving, Stable, Declining)
        /// </summary>
        [Display(Name = "Trend")]
        public string? Trend { get; set; }

        /// <summary>
        /// Variance from target
        /// </summary>
        [Display(Name = "Variance")]
        public decimal? Variance { get; set; }

        /// <summary>
        /// CSS class for styling based on status
        /// </summary>
        public string StatusCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Creator of this measurement
        /// </summary>
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Date when this measurement was recorded
        /// </summary>
        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}