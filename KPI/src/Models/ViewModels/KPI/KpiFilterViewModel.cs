using System;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for KPI filtering criteria
    /// </summary>
    public class KpiFilterViewModel
    {
        /// <summary>
        /// Search term to find KPIs by name, code, or description
        /// </summary>
        [Display(Name = "Search")]
        public string SearchTerm { get; set; } = string.Empty;

        /// <summary>
        /// Type of KPI (KRI, RI, PI)
        /// </summary>
        [Display(Name = "KPI Type")]
        public KpiType? KpiType { get; set; }

        /// <summary>
        /// Whether the indicator is a key indicator (for filtering KPIs vs PIs, or KRIs vs RIs)
        /// </summary>
        [Display(Name = "Is Key Indicator")]
        public bool? IsKey { get; set; }

        /// <summary>
        /// Category of KPI
        /// </summary>
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Department responsible for KPI
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Department name
        /// </summary>
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Status of KPI
        /// </summary>
        [Display(Name = "Status")]
        public KpiStatus? Status { get; set; }

        /// <summary>
        /// Frequency of KPI
        /// </summary>
        [Display(Name = "Frequency")]
        public string Frequency { get; set; } = string.Empty;

        /// <summary>
        /// Direction of KPI
        /// </summary>
        [Display(Name = "Direction")]
        public string Direction { get; set; } = string.Empty;

        /// <summary>
        /// Performance level filter
        /// </summary>
        [Display(Name = "Performance")]
        public int? PerformanceLevel { get; set; }

        /// <summary>
        /// Critical Success Factor ID
        /// </summary>
        [Display(Name = "Critical Success Factor")]
        public Guid? CriticalSuccessFactorId { get; set; }

        /// <summary>
        /// Field to sort by
        /// </summary>
        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "Name";

        /// <summary>
        /// Sort direction (asc/desc)
        /// </summary>
        [Display(Name = "Sort Direction")]
        public string SortDirection { get; set; } = "asc";

        /// <summary>
        /// Owner or responsible person
        /// </summary>
        [Display(Name = "Owner")]
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// Only KPIs for dashboard
        /// </summary>
        [Display(Name = "Dashboard Only")]
        public bool DashboardOnly { get; set; }

        /// <summary>
        /// Business area filter for KRIs
        /// </summary>
        [Display(Name = "Business Area")]
        public BusinessArea? BusinessArea { get; set; }

        /// <summary>
        /// Impact level filter for KRIs
        /// </summary>
        [Display(Name = "Impact Level")]
        public ImpactLevel? ImpactLevel { get; set; }

        /// <summary>
        /// Process area filter for RIs
        /// </summary>
        [Display(Name = "Process Area")]
        public ProcessArea? ProcessArea { get; set; }

        /// <summary>
        /// Activity type filter for PIs
        /// </summary>
        [Display(Name = "Activity Type")]
        public ActivityType? ActivityType { get; set; }

        /// <summary>
        /// Show only KPIs that are at risk or below target
        /// </summary>
        [Display(Name = "Show At Risk Only")]
        public bool ShowAtRiskOnly { get; set; } = false;
    }
}