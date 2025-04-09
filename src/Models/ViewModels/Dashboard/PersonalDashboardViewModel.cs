namespace KPISolution.Models.ViewModels.Dashboard
{
    /// <summary>
    /// View model for the personal dashboard
    /// </summary>
    public class PersonalDashboardViewModel
    {
        /// <summary>
        /// Gets or sets the title of the dashboard
        /// </summary>
        public string Title { get; set; } = "Dashboard cá nhân";

        /// <summary>
        /// Gets or sets the user's name
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last updated date/time
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the list of KPIs the user is responsible for
        /// </summary>
        public List<IndicatorSummaryViewModel> MyKpis { get; set; } = new List<IndicatorSummaryViewModel>();

        /// <summary>
        /// Gets or sets the list of objectives the user is responsible for
        /// </summary>
        public List<ObjectiveSummaryViewModel> MyObjectives { get; set; } = new List<ObjectiveSummaryViewModel>();

        /// <summary>
        /// Gets or sets the list of departments the user is manager of
        /// </summary>
        public List<DepartmentSummaryViewModel> MyDepartments { get; set; } = new List<DepartmentSummaryViewModel>();

        /// <summary>
        /// Gets or sets the total number of KPIs the user is responsible for
        /// </summary>
        public int TotalResponsibleKpis { get; set; }

        /// <summary>
        /// Gets or sets the total number of objectives the user is responsible for
        /// </summary>
        public int TotalObjectives { get; set; }

        /// <summary>
        /// Gets or sets the total number of departments the user is manager of
        /// </summary>
        public int TotalManagedDepartments { get; set; }

        /// <summary>
        /// Gets or sets the percentage of KPIs on target
        /// </summary>
        public decimal OnTargetPercentage { get; set; }

        /// <summary>
        /// Gets or sets the overall performance as a percentage
        /// </summary>
        public decimal OverallPerformance { get; set; }

        /// <summary>
        /// Gets or sets the CSS class for the performance display
        /// </summary>
        public string PerformanceCssClass { get; set; } = "bg-secondary";

        /// <summary>
        /// Gets the number of at-risk KPIs
        /// </summary>
        public int AtRiskKpisCount => this.MyKpis.Count(k => k.Status == IndicatorStatus.AtRisk || k.Status == IndicatorStatus.BelowTarget);

        /// <summary>
        /// Gets the number of on-target KPIs
        /// </summary>
        public int OnTargetKpisCount => this.MyKpis.Count(k => k.Status == IndicatorStatus.OnTarget);
    }
}