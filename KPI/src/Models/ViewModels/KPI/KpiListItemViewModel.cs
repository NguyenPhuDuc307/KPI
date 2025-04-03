using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    public class KpiListItemViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Person responsible for this KPI
        /// </summary>
        [Display(Name = "Responsible Person")]
        public string? ResponsiblePerson { get; set; }

        /// <summary>
        /// Parent KPI code (for RIs)
        /// </summary>
        [Display(Name = "Parent KPI")]
        public string? ParentKpiCode { get; set; }

        /// <summary>
        /// Process area (for RIs)
        /// </summary>
        [Display(Name = "Process Area")]
        public string? ProcessArea { get; set; }

        /// <summary>
        /// Measurement unit (e.g., %, count, time)
        /// </summary>
        [Display(Name = "Unit")]
        public string? MeasurementUnit { get; set; }

        /// <summary>
        /// Target value
        /// </summary>
        [Display(Name = "Target")]
        public decimal TargetValue { get; set; }

        /// <summary>
        /// Current value
        /// </summary>
        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }

        /// <summary>
        /// KPI Type (KRI, RI, PI)
        /// </summary>
        [Display(Name = "KPI Type")]
        public KpiType KpiType { get; set; }

        /// <summary>
        /// KPI Status
        /// </summary>
        [Display(Name = "Status")]
        public KpiStatus Status { get; set; }

        /// <summary>
        /// KPI Status as a string
        /// </summary>
        [Display(Name = "Status")]
        public string StatusString { get; set; } = string.Empty;

        /// <summary>
        /// Activity type name for performance indicators
        /// </summary>
        [Display(Name = "Activity Type")]
        public string? ActivityTypeName { get; set; }

        /// <summary>
        /// Performance rating
        /// </summary>
        [Display(Name = "Performance")]
        public string? PerformanceRating { get; set; }

        /// <summary>
        /// Strategic objective
        /// </summary>
        [Display(Name = "Strategic Objective")]
        public string? StrategicObjective { get; set; }

        /// <summary>
        /// Executive owner
        /// </summary>
        [Display(Name = "Executive Owner")]
        public string? ExecutiveOwner { get; set; }

        /// <summary>
        /// Trend direction
        /// </summary>
        [Display(Name = "Trend")]
        public TrendDirection? TrendDirection { get; set; }

        /// <summary>
        /// Trend value
        /// </summary>
        [Display(Name = "Trend Value")]
        public decimal? TrendValue { get; set; }

        /// <summary>
        /// Last updated date
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime? LastUpdated { get; set; }
    }
}