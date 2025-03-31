using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.Dashboard
{
    /// <summary>
    /// View model for the executive dashboard displaying high-level KPI information
    /// </summary>
    public class ExecutiveDashboardViewModel
    {
        /// <summary>
        /// Constructor to initialize collections
        /// </summary>
        public ExecutiveDashboardViewModel()
        {
            KriSummaries = new List<KpiSummaryViewModel>();
            CsfSummaries = new List<CsfSummaryViewModel>();
            PerformanceByDepartment = new List<DepartmentPerformanceViewModel>();
            KpisByStatus = new Dictionary<KpiStatus, int>();
            KpisByCategory = new Dictionary<KpiCategory, int>();
            RecentUpdates = new List<KpiUpdateViewModel>();
            AlertsRequiringAttention = new List<KpiAlertViewModel>();
        }

        /// <summary>
        /// Title of the dashboard
        /// </summary>
        [Display(Name = "Dashboard Title")]
        public string Title { get; set; } = "Executive Dashboard";

        /// <summary>
        /// Date/time when the dashboard was last refreshed
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Key Result Indicators summaries
        /// </summary>
        [Display(Name = "Key Performance Indicators")]
        public List<KpiSummaryViewModel> KriSummaries { get; set; } = new List<KpiSummaryViewModel>();

        /// <summary>
        /// Critical Success Factors summaries
        /// </summary>
        [Display(Name = "Critical Success Factors")]
        public List<CsfSummaryViewModel> CsfSummaries { get; set; } = new List<CsfSummaryViewModel>();

        /// <summary>
        /// Overall organization performance percentage
        /// </summary>
        [Display(Name = "Overall Performance")]
        public decimal OverallPerformance { get; set; }

        /// <summary>
        /// CSS class for the overall performance indicator
        /// </summary>
        public string OverallPerformanceClass { get; set; } = string.Empty;

        /// <summary>
        /// Performance metrics by department
        /// </summary>
        [Display(Name = "Department Performance")]
        public List<DepartmentPerformanceViewModel> PerformanceByDepartment { get; set; } = new List<DepartmentPerformanceViewModel>();

        /// <summary>
        /// Counts of KPIs by status
        /// </summary>
        [Display(Name = "KPIs by Status")]
        public Dictionary<KpiStatus, int> KpisByStatus { get; set; } = new Dictionary<KpiStatus, int>();

        /// <summary>
        /// Counts of KPIs by category
        /// </summary>
        [Display(Name = "KPIs by Category")]
        public Dictionary<KpiCategory, int> KpisByCategory { get; set; } = new Dictionary<KpiCategory, int>();

        /// <summary>
        /// Total count of all KPIs
        /// </summary>
        [Display(Name = "Total KPIs")]
        public int TotalKpiCount { get; set; }

        /// <summary>
        /// Count of KPIs currently at risk
        /// </summary>
        [Display(Name = "At Risk KPIs")]
        public int AtRiskKpiCount { get; set; }

        /// <summary>
        /// Percentage of KPIs that are on target
        /// </summary>
        [Display(Name = "On Target (%)")]
        public decimal OnTargetPercentage { get; set; }

        /// <summary>
        /// Recent KPI updates for the activity feed
        /// </summary>
        [Display(Name = "Recent Updates")]
        public List<KpiUpdateViewModel> RecentUpdates { get; set; } = new List<KpiUpdateViewModel>();

        /// <summary>
        /// KPI alerts requiring attention
        /// </summary>
        [Display(Name = "Alerts")]
        public List<KpiAlertViewModel> AlertsRequiringAttention { get; set; } = new List<KpiAlertViewModel>();

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
    }

    /// <summary>
    /// Summary view model for KPIs in the dashboard
    /// </summary>
    public class KpiSummaryViewModel
    {
        /// <summary>
        /// KPI ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// KPI name
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// KPI code identifier
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Current value of the KPI
        /// </summary>
        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }

        /// <summary>
        /// Target value for the KPI
        /// </summary>
        [Display(Name = "Target")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Measurement unit
        /// </summary>
        [Display(Name = "Unit")]
        public string MeasurementUnit { get; set; } = string.Empty;

        /// <summary>
        /// Percentage of target achieved
        /// </summary>
        [Display(Name = "% of Target")]
        public decimal? PercentageOfTarget { get; set; }

        /// <summary>
        /// Date of the last measurement
        /// </summary>
        [Display(Name = "Last Updated")]
        [DataType(DataType.DateTime)]
        public DateTime? MeasurementDate { get; set; }

        /// <summary>
        /// Status of the KPI
        /// </summary>
        [Display(Name = "Status")]
        public KpiStatus Status { get; set; }

        /// <summary>
        /// Status as a string
        /// </summary>
        [Display(Name = "Status")]
        public string StatusDisplay { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for styling based on status
        /// </summary>
        public string StatusCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Trend direction
        /// </summary>
        [Display(Name = "Trend")]
        public TrendDirection Trend { get; set; }

        /// <summary>
        /// Trend as a string
        /// </summary>
        [Display(Name = "Trend")]
        public string TrendDisplay { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for styling based on trend
        /// </summary>
        public string TrendCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Percentage change from previous measurement
        /// </summary>
        [Display(Name = "Change")]
        public decimal? PercentageChange { get; set; }

        /// <summary>
        /// Trend value display for UI
        /// </summary>
        [Display(Name = "Trend Value")]
        public string TrendValueDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Department responsible for the KPI
        /// </summary>
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;
    }

    /// <summary>
    /// Summary view model for CSFs in the dashboard
    /// </summary>
    public class CsfSummaryViewModel
    {
        /// <summary>
        /// CSF ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// CSF name
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// CSF code identifier
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Current progress percentage
        /// </summary>
        [Display(Name = "Progress")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// CSS class for progress styling
        /// </summary>
        public string ProgressCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Target date for completion
        /// </summary>
        [Display(Name = "Target Date")]
        [DataType(DataType.Date)]
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Days remaining until target date
        /// </summary>
        [Display(Name = "Days Remaining")]
        public int DaysRemaining { get; set; }

        /// <summary>
        /// Whether the CSF is on track
        /// </summary>
        [Display(Name = "On Track")]
        public bool IsOnTrack { get; set; }

        /// <summary>
        /// Status of the CSF
        /// </summary>
        [Display(Name = "Status")]
        public CSFStatus Status { get; set; }

        /// <summary>
        /// Status as a string
        /// </summary>
        [Display(Name = "Status")]
        public string StatusDisplay { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for styling based on status
        /// </summary>
        public string StatusCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Category of the CSF
        /// </summary>
        [Display(Name = "Category")]
        public CSFCategory Category { get; set; }

        /// <summary>
        /// Category as a string
        /// </summary>
        [Display(Name = "Category")]
        public string CategoryDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Priority level of the CSF
        /// </summary>
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; }

        /// <summary>
        /// Priority as a string
        /// </summary>
        [Display(Name = "Priority")]
        public string PriorityDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Department responsible for the CSF
        /// </summary>
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;
    }

    /// <summary>
    /// Performance summary for a department
    /// </summary>
    public class DepartmentPerformanceViewModel
    {
        /// <summary>
        /// Department ID
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Department name
        /// </summary>
        [Display(Name = "Department")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Performance score as a percentage
        /// </summary>
        [Display(Name = "Performance")]
        public decimal PerformanceScore { get; set; }

        /// <summary>
        /// Performance percentage for UI display
        /// </summary>
        [Display(Name = "Performance")]
        public decimal PerformancePercentage { get; set; }

        /// <summary>
        /// CSS class for performance styling
        /// </summary>
        public string PerformanceCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Number of KPIs owned by the department
        /// </summary>
        [Display(Name = "KPI Count")]
        public int KpiCount { get; set; }

        /// <summary>
        /// Number of KPIs at risk
        /// </summary>
        [Display(Name = "At Risk")]
        public int AtRiskCount { get; set; }

        /// <summary>
        /// Percentage of KPIs on target
        /// </summary>
        [Display(Name = "On Target")]
        public decimal OnTargetPercentage { get; set; }
    }

    /// <summary>
    /// Recent KPI update for activity feed
    /// </summary>
    public class KpiUpdateViewModel
    {
        /// <summary>
        /// KPI ID
        /// </summary>
        public Guid KpiId { get; set; }

        /// <summary>
        /// KPI name
        /// </summary>
        [Display(Name = "KPI")]
        public string KpiName { get; set; } = string.Empty;

        /// <summary>
        /// Previous value
        /// </summary>
        [Display(Name = "Previous Value")]
        public decimal? PreviousValue { get; set; }

        /// <summary>
        /// New value
        /// </summary>
        [Display(Name = "New Value")]
        public decimal? NewValue { get; set; }

        /// <summary>
        /// Percentage change
        /// </summary>
        [Display(Name = "Change")]
        public decimal? PercentageChange { get; set; }

        /// <summary>
        /// CSS class for change indicator
        /// </summary>
        public string ChangeCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Date/time of the update
        /// </summary>
        [Display(Name = "Updated On")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// User who updated the KPI
        /// </summary>
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// KPI alert for dashboard notifications
    /// </summary>
    public class KpiAlertViewModel
    {
        /// <summary>
        /// Alert ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// KPI ID
        /// </summary>
        public Guid KpiId { get; set; }

        /// <summary>
        /// KPI name
        /// </summary>
        [Display(Name = "KPI")]
        public string KpiName { get; set; } = string.Empty;

        /// <summary>
        /// Alert message
        /// </summary>
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Alert severity
        /// </summary>
        [Display(Name = "Severity")]
        public AlertSeverity Severity { get; set; }

        /// <summary>
        /// Severity as a string
        /// </summary>
        [Display(Name = "Severity")]
        public string SeverityDisplay { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for severity styling
        /// </summary>
        public string SeverityCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Date/time when the alert was created
        /// </summary>
        [Display(Name = "Created On")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}