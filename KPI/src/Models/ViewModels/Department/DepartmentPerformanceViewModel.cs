using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.ViewModels.KPI;

namespace KPISolution.Models.ViewModels.Department
{
    /// <summary>
    /// View model for department performance information.
    /// </summary>
    public class DepartmentPerformanceViewModel
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
        /// Gets or sets the overall performance score.
        /// </summary>
        [Display(Name = "Overall Performance")]
        public decimal OverallScore { get; set; }

        /// <summary>
        /// Gets or sets the KPI achievement rate.
        /// </summary>
        [Display(Name = "KPI Achievement Rate")]
        public decimal KpiAchievementRate { get; set; }

        /// <summary>
        /// Gets or sets the number of KPIs on track.
        /// </summary>
        [Display(Name = "KPIs On Track")]
        public int KpisOnTrack { get; set; }

        /// <summary>
        /// Gets or sets the number of KPIs at risk.
        /// </summary>
        [Display(Name = "KPIs At Risk")]
        public int KpisAtRisk { get; set; }

        /// <summary>
        /// Gets or sets the number of KPIs off track.
        /// </summary>
        [Display(Name = "KPIs Off Track")]
        public int KpisOffTrack { get; set; }

        /// <summary>
        /// Gets or sets the performance trend over time.
        /// </summary>
        [Display(Name = "Performance Trend")]
        public string PerformanceTrend { get; set; }

        /// <summary>
        /// Gets or sets the list of top performing KPIs.
        /// </summary>
        [Display(Name = "Top Performing KPIs")]
        public List<KpiViewModel> TopPerformingKpis { get; set; }

        /// <summary>
        /// Gets or sets the list of underperforming KPIs.
        /// </summary>
        [Display(Name = "Underperforming KPIs")]
        public List<KpiViewModel> UnderperformingKpis { get; set; }

        /// <summary>
        /// Gets or sets the performance by KPI type breakdown.
        /// </summary>
        [Display(Name = "Performance by KPI Type")]
        public Dictionary<string, decimal> PerformanceByType { get; set; }

        /// <summary>
        /// Gets or sets the historical performance data.
        /// </summary>
        [Display(Name = "Historical Performance")]
        public Dictionary<DateTime, decimal> HistoricalPerformance { get; set; }

        /// <summary>
        /// Gets or sets the last updated timestamp.
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets any performance improvement recommendations.
        /// </summary>
        [Display(Name = "Recommendations")]
        public List<string> Recommendations { get; set; }

        /// <summary>
        /// Initializes a new instance of the DepartmentPerformanceViewModel class.
        /// </summary>
        public DepartmentPerformanceViewModel()
        {
            DepartmentName = string.Empty;
            PerformanceTrend = string.Empty;
            TopPerformingKpis = new List<KpiViewModel>();
            UnderperformingKpis = new List<KpiViewModel>();
            PerformanceByType = new Dictionary<string, decimal>();
            HistoricalPerformance = new Dictionary<DateTime, decimal>();
            Recommendations = new List<string>();
        }
    }
}