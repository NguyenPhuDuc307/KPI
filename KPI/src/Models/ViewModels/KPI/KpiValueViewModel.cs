using System;
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
        /// Code of the KPI for display purposes
        /// </summary>
        [Display(Name = "Mã KPI")]
        public string KpiCode { get; set; } = string.Empty;

        /// <summary>
        /// Name of the KPI
        /// </summary>
        [Display(Name = "Tên KPI")]
        public string KpiName { get; set; } = string.Empty;

        /// <summary>
        /// Type of KPI (KRI, RI, PI)
        /// </summary>
        [Display(Name = "KPI Type")]
        public string KpiType { get; set; } = string.Empty;

        /// <summary>
        /// The actual measured value
        /// </summary>
        [Display(Name = "Giá trị")]
        public decimal ActualValue { get; set; }

        /// <summary>
        /// The target value for comparison
        /// </summary>
        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Date when the measurement was taken
        /// </summary>
        [Display(Name = "Ngày đo")]
        [DataType(DataType.Date)]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// The period this measurement belongs to (e.g., Jan 2023, Q1 2023)
        /// </summary>
        [Display(Name = "Period")]
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// Additional notes about the measurement
        /// </summary>
        [Display(Name = "Ghi chú")]
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
        /// User who created the measurement
        /// </summary>
        [Display(Name = "Người tạo")]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the measurement was created
        /// </summary>
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// User who last updated the measurement
        /// </summary>
        [Display(Name = "Người cập nhật")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Date and time when the measurement was last updated
        /// </summary>
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }
    }
}