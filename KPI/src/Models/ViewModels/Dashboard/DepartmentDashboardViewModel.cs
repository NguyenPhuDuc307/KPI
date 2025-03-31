using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;
using System;
using System.Collections.Generic;

namespace KPISolution.Models.ViewModels.Dashboard
{
    /// <summary>
    /// View model for department-specific dashboard
    /// </summary>
    public class DepartmentDashboardViewModel
    {
        /// <summary>
        /// Constructor to initialize collections
        /// </summary>
        public DepartmentDashboardViewModel()
        {
            KpiSummaries = new List<KpiSummaryViewModel>();
            LinkedCsfs = new List<CsfSummaryViewModel>();
            TeamPerformance = new List<TeamMemberPerformanceViewModel>();
            KpisRequiringUpdate = new List<KpiUpdateRequiredViewModel>();
            RecentUpdates = new List<KpiUpdateViewModel>();
            PerformanceOverTime = new Dictionary<DateTime, decimal>();
        }

        /// <summary>
        /// Department ID
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Department name
        /// </summary>
        [Display(Name = "Department")]
        public required string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Department manager
        /// </summary>
        [Display(Name = "Manager")]
        public string Manager { get; set; } = string.Empty;

        /// <summary>
        /// Date when the dashboard was last refreshed
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// All KPI summaries for the department
        /// </summary>
        [Display(Name = "Key Performance Indicators")]
        public List<KpiSummaryViewModel> KpiSummaries { get; set; } = new List<KpiSummaryViewModel>();

        /// <summary>
        /// Critical Success Factors linked to department KPIs
        /// </summary>
        [Display(Name = "Critical Success Factors")]
        public List<CsfSummaryViewModel> LinkedCsfs { get; set; } = new List<CsfSummaryViewModel>();

        /// <summary>
        /// Overall department performance percentage
        /// </summary>
        [Display(Name = "Department Performance")]
        public decimal OverallPerformance { get; set; }

        /// <summary>
        /// CSS class for styling based on performance
        /// </summary>
        public string PerformanceCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Total number of KPIs owned by the department
        /// </summary>
        [Display(Name = "Total KPIs")]
        public int TotalKpiCount { get; set; }

        /// <summary>
        /// Count of KPIs at risk (below target)
        /// </summary>
        [Display(Name = "At Risk KPIs")]
        public int AtRiskKpiCount { get; set; }

        /// <summary>
        /// Percentage of KPIs that are on target
        /// </summary>
        [Display(Name = "On Target (%)")]
        public decimal OnTargetPercentage { get; set; }

        /// <summary>
        /// Number of KPIs due for update
        /// </summary>
        [Display(Name = "Updates Due")]
        public int UpdatesDueCount { get; set; }

        /// <summary>
        /// Performance scores for team members
        /// </summary>
        [Display(Name = "Team Performance")]
        public List<TeamMemberPerformanceViewModel> TeamPerformance { get; set; } = new List<TeamMemberPerformanceViewModel>();

        /// <summary>
        /// KPIs that need to be updated
        /// </summary>
        [Display(Name = "KPIs Requiring Updates")]
        public List<KpiUpdateRequiredViewModel> KpisRequiringUpdate { get; set; } = new List<KpiUpdateRequiredViewModel>();

        /// <summary>
        /// Recent KPI updates
        /// </summary>
        [Display(Name = "Recent Updates")]
        public List<KpiUpdateViewModel> RecentUpdates { get; set; } = new List<KpiUpdateViewModel>();

        /// <summary>
        /// Filtering period start date
        /// </summary>
        [Display(Name = "Period Start")]
        [DataType(DataType.Date)]
        public DateTime? PeriodStart { get; set; }

        /// <summary>
        /// Filtering period end date
        /// </summary>
        [Display(Name = "Period End")]
        [DataType(DataType.Date)]
        public DateTime? PeriodEnd { get; set; }

        /// <summary>
        /// Department performance over time (for charts)
        /// </summary>
        public Dictionary<DateTime, decimal> PerformanceOverTime { get; set; } = new Dictionary<DateTime, decimal>();
    }

    /// <summary>
    /// Performance summary for a team member
    /// </summary>
    public class TeamMemberPerformanceViewModel
    {
        /// <summary>
        /// User ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Team member name
        /// </summary>
        [Display(Name = "Team Member")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Position or title
        /// </summary>
        [Display(Name = "Position")]
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// Number of KPIs owned by this team member
        /// </summary>
        [Display(Name = "KPI Count")]
        public int KpiCount { get; set; }

        /// <summary>
        /// Performance score as a percentage
        /// </summary>
        [Display(Name = "Performance")]
        public decimal PerformanceScore { get; set; }

        /// <summary>
        /// CSS class for styling based on performance
        /// </summary>
        public string PerformanceCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Number of KPIs at risk
        /// </summary>
        [Display(Name = "At Risk")]
        public int AtRiskCount { get; set; }

        /// <summary>
        /// Number of updates due
        /// </summary>
        [Display(Name = "Updates Due")]
        public int UpdatesDueCount { get; set; }
    }

    /// <summary>
    /// KPI that requires an update
    /// </summary>
    public class KpiUpdateRequiredViewModel
    {
        /// <summary>
        /// KPI ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// KPI name
        /// </summary>
        [Display(Name = "KPI")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// KPI code
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Owner responsible for the KPI
        /// </summary>
        [Display(Name = "Owner")]
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// Measurement frequency
        /// </summary>
        [Display(Name = "Frequency")]
        public MeasurementFrequency Frequency { get; set; }

        /// <summary>
        /// Frequency as a string
        /// </summary>
        [Display(Name = "Frequency")]
        public string FrequencyDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Date of last measurement
        /// </summary>
        [Display(Name = "Last Updated")]
        [DataType(DataType.Date)]
        public DateTime? LastUpdatedDate { get; set; }

        /// <summary>
        /// Due date for next measurement
        /// </summary>
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Days overdue (negative means due in the future)
        /// </summary>
        [Display(Name = "Days Overdue")]
        public int DaysOverdue { get; set; }

        /// <summary>
        /// CSS class for styling based on urgency
        /// </summary>
        public string UrgencyCssClass { get; set; } = string.Empty;
    }
}