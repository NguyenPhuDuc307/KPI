namespace KPISolution.Models.ViewModels.Dashboard
{
    /// <summary>
    /// View model for the executive dashboard displaying high-level Indicator information
    /// </summary>
    public class ExecutiveDashboardViewModel
    {
        /// <summary>
        /// Constructor to initialize collections
        /// </summary>
        public ExecutiveDashboardViewModel()
        {
            this.KriSummaries = [];
            this.SuccessFactorSummaries = [];
            this.PerformanceByDepartment = [];
            this.IndicatorsByStatus = new Dictionary<IndicatorStatus, int>();
            this.IndicatorsByCategory = new Dictionary<string, int>();
            this.RecentUpdates = [];
            this.AlertsRequiringAttention = [];
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
        public List<ExecutiveIndicatorSummaryViewModel> KriSummaries { get; set; } = [];

        /// <summary>
        /// Critical Success Factors summaries
        /// </summary>
        [Display(Name = "Critical Success Factors")]
        public List<ExecutiveSuccessFactorSummaryViewModel> SuccessFactorSummaries { get; set; } = [];

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
        public List<DepartmentPerformanceViewModel> PerformanceByDepartment { get; set; } = [];

        /// <summary>
        /// Counts of Indicators by status
        /// </summary>
        [Display(Name = "Indicators by Status")]
        public Dictionary<IndicatorStatus, int> IndicatorsByStatus { get; set; } = new Dictionary<IndicatorStatus, int>();

        /// <summary>
        /// Counts of Indicators by category
        /// </summary>
        [Display(Name = "Indicators by Category")]
        public Dictionary<string, int> IndicatorsByCategory { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Total count of all Indicators
        /// </summary>
        [Display(Name = "Total Indicators")]
        public int TotalIndicatorCount { get; set; }

        /// <summary>
        /// Count of Indicators currently at risk
        /// </summary>
        [Display(Name = "At Risk Indicators")]
        public int AtRiskIndicatorCount { get; set; }

        /// <summary>
        /// Percentage of Indicators that are on target
        /// </summary>
        [Display(Name = "On Target (%)")]
        public decimal OnTargetPercentage { get; set; }

        /// <summary>
        /// Recent Indicator updates for the activity feed
        /// </summary>
        [Display(Name = "Recent Updates")]
        public List<IndicatorUpdateViewModel> RecentUpdates { get; set; } = [];

        /// <summary>
        /// Indicator alerts requiring attention
        /// </summary>
        [Display(Name = "Alerts")]
        public List<IndicatorAlertViewModel> AlertsRequiringAttention { get; set; } = [];

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
        /// Total count of business objectives
        /// </summary>
        [Display(Name = "Objectives")]
        public int ObjectiveCount { get; set; }

        /// <summary>
        /// Total count of success factors
        /// </summary>
        [Display(Name = "Success Factors")]
        public int SfCount { get; set; }

        /// <summary>
        /// Total count of critical success factors
        /// </summary>
        [Display(Name = "Critical Success Factors")]
        public int SuccessFactorCount { get; set; }

        /// <summary>
        /// Total count of result and performance indicators
        /// </summary>
        [Display(Name = "Indicators")]
        public int IndicatorCount { get; set; }

        /// <summary>
        /// Total count of key result and performance indicators
        /// </summary>
        [Display(Name = "Key Indicators")]
        public int KeyIndicatorCount { get; set; }

        /// <summary>
        /// Current page for department listing
        /// </summary>
        [Display(Name = "Department Page")]
        public int DepartmentPage { get; set; } = 1;

        /// <summary>
        /// Current page for KRI listing
        /// </summary>
        [Display(Name = "KRI Page")]
        public int KriPage { get; set; } = 1;

        /// <summary>
        /// Summaries of objectives for display in dashboard
        /// </summary>
        [Display(Name = "Objectives")]
        public List<ObjectiveSummaryViewModel> ObjectiveSummaries { get; set; } = [];
    }

    /// <summary>
    /// Summary view model for Indicators in the dashboard
    /// </summary>
    public class ExecutiveIndicatorSummaryViewModel
    {
        /// <summary>
        /// Indicator ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Indicator name
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Indicator code identifier
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Current value of the Indicator
        /// </summary>
        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }

        /// <summary>
        /// Target value for the Indicator
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
        /// Status of the Indicator
        /// </summary>
        [Display(Name = "Status")]
        public IndicatorStatus Status { get; set; }

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
        /// Department responsible for the Indicator
        /// </summary>
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;
    }

    /// <summary>
    /// Summary of Critical Success Factors for executive dashboard
    /// </summary>
    public class ExecutiveSuccessFactorSummaryViewModel
    {
        /// <summary>
        /// SuccessFactor ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// SuccessFactor name
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// SuccessFactor code identifier
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
        /// Whether the SuccessFactor is on track
        /// </summary>
        [Display(Name = "On Track")]
        public bool IsOnTrack { get; set; }

        /// <summary>
        /// Status of the SuccessFactor
        /// </summary>
        [Display(Name = "Status")]
        public SuccessFactorStatus Status { get; set; }

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
        /// Category of the SuccessFactor
        /// </summary>
        [Display(Name = "Category")]
        public string CategoryDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Priority level of the SuccessFactor
        /// </summary>
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; }

        /// <summary>
        /// Priority as a string
        /// </summary>
        [Display(Name = "Priority")]
        public string PriorityDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Department responsible for the SuccessFactor
        /// </summary>
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;
    }

    /// <summary>
    /// Recent Indicator update for activity feed
    /// </summary>
    public class IndicatorUpdateViewModel
    {
        /// <summary>
        /// Indicator ID
        /// </summary>
        public Guid IndicatorId { get; set; }

        /// <summary>
        /// Indicator name
        /// </summary>
        [Display(Name = "Indicator")]
        public string IndicatorName { get; set; } = string.Empty;

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
        /// User who updated the Indicator
        /// </summary>
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Indicator alert for dashboard notifications
    /// </summary>
    public class IndicatorAlertViewModel
    {
        /// <summary>
        /// Alert ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Indicator ID
        /// </summary>
        public Guid IndicatorId { get; set; }

        /// <summary>
        /// Indicator name
        /// </summary>
        [Display(Name = "Indicator")]
        public string IndicatorName { get; set; } = string.Empty;

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

    /// <summary>
    /// Summary view model for objectives in the dashboard
    /// </summary>
    public class ObjectiveSummaryViewModel
    {
        /// <summary>
        /// Objective ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Objective code
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Objective name
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Department name
        /// </summary>
        [Display(Name = "Department")]
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Progress percentage
        /// </summary>
        [Display(Name = "Progress")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// CSS class for the progress bar
        /// </summary>
        public string ProgressCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Count of success factors
        /// </summary>
        [Display(Name = "SFs")]
        public int SuccessFactorCount { get; set; }

        /// <summary>
        /// Count of critical success factors
        /// </summary>
        [Display(Name = "Critical Success Factors")]
        public int CriticalSuccessFactorCount { get; set; }

        /// <summary>
        /// Count of Indicators linked to this objective
        /// </summary>
        [Display(Name = "Indicators")]
        public int IndicatorCount { get; set; }
    }
}
