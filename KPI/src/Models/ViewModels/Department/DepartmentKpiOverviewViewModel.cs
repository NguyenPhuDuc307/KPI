using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.ViewModels.KPI;

namespace KPISolution.Models.ViewModels.Department
{
    /// <summary>
    /// View model for department KPI overview information.
    /// </summary>
    public class DepartmentKpiOverviewViewModel
    {
        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the department name.
        /// </summary>
        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the total number of KPIs.
        /// </summary>
        [Display(Name = "Total KPIs")]
        public int TotalKpis { get; set; }

        /// <summary>
        /// Gets or sets the number of active KPIs.
        /// </summary>
        [Display(Name = "Active KPIs")]
        public int ActiveKpis { get; set; }

        /// <summary>
        /// Gets or sets the number of completed KPIs.
        /// </summary>
        [Display(Name = "Completed KPIs")]
        public int CompletedKpis { get; set; }

        /// <summary>
        /// Gets or sets the KPIs by type breakdown.
        /// </summary>
        [Display(Name = "KPIs by Type")]
        public Dictionary<string, int> KpisByType { get; set; }

        /// <summary>
        /// Gets or sets the KPIs by status breakdown.
        /// </summary>
        [Display(Name = "KPIs by Status")]
        public Dictionary<string, int> KpisByStatus { get; set; }

        /// <summary>
        /// Gets or sets the list of recent KPI updates.
        /// </summary>
        [Display(Name = "Recent Updates")]
        public List<KpiValueViewModel> RecentUpdates { get; set; }

        /// <summary>
        /// Gets or sets the list of upcoming KPI measurements.
        /// </summary>
        [Display(Name = "Upcoming Measurements")]
        public List<KpiViewModel> UpcomingMeasurements { get; set; }

        /// <summary>
        /// Gets or sets the list of overdue KPI measurements.
        /// </summary>
        [Display(Name = "Overdue Measurements")]
        public List<KpiViewModel> OverdueMeasurements { get; set; }

        /// <summary>
        /// Gets or sets the achievement rate by KPI type.
        /// </summary>
        [Display(Name = "Achievement Rate by Type")]
        public Dictionary<string, decimal> AchievementRateByType { get; set; }

        /// <summary>
        /// Gets or sets the measurement frequency breakdown.
        /// </summary>
        [Display(Name = "Measurement Frequency")]
        public Dictionary<string, int> MeasurementFrequency { get; set; }

        /// <summary>
        /// Gets or sets the data quality score.
        /// </summary>
        [Display(Name = "Data Quality Score")]
        public decimal DataQualityScore { get; set; }

        /// <summary>
        /// Gets or sets the last updated timestamp.
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Initializes a new instance of the DepartmentKpiOverviewViewModel class.
        /// </summary>
        public DepartmentKpiOverviewViewModel()
        {
            DepartmentName = string.Empty;
            KpisByType = new Dictionary<string, int>();
            KpisByStatus = new Dictionary<string, int>();
            RecentUpdates = new List<KpiValueViewModel>();
            UpcomingMeasurements = new List<KpiViewModel>();
            OverdueMeasurements = new List<KpiViewModel>();
            AchievementRateByType = new Dictionary<string, decimal>();
            MeasurementFrequency = new Dictionary<string, int>();
        }
    }
}