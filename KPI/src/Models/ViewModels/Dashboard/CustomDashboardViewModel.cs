namespace KPISolution.Models.ViewModels.Dashboard
{
    /// <summary>
    /// View model for custom user dashboards allowing personalized Indicator monitoring
    /// </summary>
    public class CustomDashboardViewModel
    {
        /// <summary>
        /// Constructor to initialize collections
        /// </summary>
        public CustomDashboardViewModel()
        {
            this.DashboardItems = [];
            this.AvailableIndicators = [];
            this.SavedLayouts = [];
            this.SharedUsers = [];
            this.AvailableUsers = [];
            this.AvailableSuccessFactors = [];
        }

        /// <summary>
        /// Dashboard ID
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Title of the dashboard
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        [Display(Name = "Dashboard Title")]
        public required string Title { get; init; } = string.Empty;

        /// <summary>
        /// Description of the dashboard
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// User ID who owns this dashboard
        /// </summary>
        public required string UserId { get; init; } = string.Empty;

        /// <summary>
        /// Username who owns this dashboard
        /// </summary>
        [Display(Name = "Owner")]
        public required string UserName { get; init; } = string.Empty;

        /// <summary>
        /// Date/time when the dashboard was last refreshed
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Date/time when the dashboard configuration was last modified
        /// </summary>
        [Display(Name = "Last Modified")]
        public DateTime LastModified { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Items shown on this dashboard
        /// </summary>
        public List<DashboardItemViewModel> DashboardItems { get; init; } = [];

        /// <summary>
        /// Alias for DashboardItems for backward compatibility
        /// </summary>
        public List<DashboardItemViewModel> Items => this.DashboardItems;

        /// <summary>
        /// Indicators available to add to the dashboard
        /// </summary>
        [Display(Name = "Available Indicators")]
        public List<IndicatorSummaryViewModel> AvailableIndicators { get; set; } = [];

        /// <summary>
        /// SuccessFactors available to add to the dashboard
        /// </summary>
        [Display(Name = "Available Success Factors")]
        public List<SuccessFactorSummaryViewModel> AvailableSuccessFactors { get; init; } = [];

        /// <summary>
        /// Saved dashboard layouts
        /// </summary>
        [Display(Name = "Saved Layouts")]
        public List<DashboardLayoutViewModel> SavedLayouts { get; init; }

        /// <summary>
        /// Layout configuration (JSON)
        /// </summary>
        public string? LayoutConfiguration { get; init; }

        /// <summary>
        /// Whether this dashboard is the user's default
        /// </summary>
        [Display(Name = "Set as Default")]
        public bool IsDefault { get; init; }

        /// <summary>
        /// Whether this dashboard is shared with others
        /// </summary>
        [Display(Name = "Share Dashboard")]
        public bool IsShared { get; init; }

        /// <summary>
        /// Whether this dashboard is public (visible to all users)
        /// </summary>
        [Display(Name = "Public Dashboard")]
        public bool IsPublic { get; init; }

        /// <summary>
        /// Users this dashboard is shared with
        /// </summary>
        [Display(Name = "Shared With")]
        public List<DashboardUserViewModel> SharedUsers { get; init; } = [];

        /// <summary>
        /// Users available for sharing
        /// </summary>
        [Display(Name = "Available Users")]
        public List<DashboardUserViewModel> AvailableUsers { get; init; } = [];

        /// <summary>
        /// Refresh interval in minutes
        /// </summary>
        [Display(Name = "Auto-refresh (minutes)")]
        [Range(0, 1440, ErrorMessage = "Refresh interval must be between 0 and 1440 minutes")]
        public int RefreshInterval { get; init; }
    }

    /// <summary>
    /// Individual item on a custom dashboard
    /// </summary>
    public class DashboardItemViewModel
    {
        /// <summary>
        /// Item ID
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Indicator ID
        /// </summary>
        public Guid? IndicatorId { get; init; }

        /// <summary>
        /// SuccessFactor ID if this item displays a Success Factor
        /// </summary>
        public Guid? SuccessFactorId { get; init; }

        /// <summary>
        /// Chart type for displaying this item
        /// </summary>
        [Display(Name = "Chart Type")]
        public ChartType ChartType { get; init; } = ChartType.Card;

        /// <summary>
        /// Title of the item
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Title")]
        public string Title { get; init; } = string.Empty;

        /// <summary>
        /// Width of the item (1-12 column grid)
        /// </summary>
        [Range(1, 12)]
        [Display(Name = "Width")]
        public int Width { get; init; } = 4;

        /// <summary>
        /// Height of the item (1-12 row grid)
        /// </summary>
        [Range(1, 12)]
        [Display(Name = "Height")]
        public int Height { get; init; } = 4;

        /// <summary>
        /// X position (column)
        /// </summary>
        [Range(0, 11)]
        public int X { get; init; }

        /// <summary>
        /// Y position (row)
        /// </summary>
        [Range(0, 11)]
        public int Y { get; init; }

        /// <summary>
        /// Data configuration (JSON)
        /// </summary>
        public string DataConfiguration { get; init; } = string.Empty;

        /// <summary>
        /// Order in which the item appears (for non-grid layouts)
        /// </summary>
        public int Order { get; init; }

        /// <summary>
        /// Item type (Indicator, SuccessFactor, custom)
        /// </summary>
        public DashboardItemType ItemType { get; init; } = DashboardItemType.Chart;

        /// <summary>
        /// Whether to show legends on charts
        /// </summary>
        [Display(Name = "Show Legend")]
        public bool ShowLegend { get; init; } = true;

        /// <summary>
        /// Time period to display
        /// </summary>
        [Display(Name = "Time Period")]
        public TimePeriod TimePeriod { get; init; } = TimePeriod.Last30Days;

        /// <summary>
        /// Widget type for display
        /// </summary>
        [Display(Name = "Widget Type")]
        public string WidgetType { get; set; } = string.Empty;

        /// <summary>
        /// Widget data for display
        /// </summary>
        public object? WidgetData { get; set; }
    }

    /// <summary>
    /// User for dashboard sharing
    /// </summary>
    public class DashboardUserViewModel
    {
        /// <summary>
        /// User ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// User name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Department name
        /// </summary>
        public string Department { get; set; } = string.Empty;
    }

    /// <summary>
    /// Saved dashboard layout configuration
    /// </summary>
    public class DashboardLayoutViewModel
    {
        /// <summary>
        /// Layout ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the layout
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Layout Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the layout
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Date when the layout was created
        /// </summary>
        [Display(Name = "Created On")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Layout configuration (JSON)
        /// </summary>
        public string Configuration { get; set; } = string.Empty;

        /// <summary>
        /// Whether this is a system template available to all users
        /// </summary>
        [Display(Name = "System Template")]
        public bool IsSystemTemplate { get; set; }
    }

    /// <summary>
    /// Time periods for data filtering
    /// </summary>
    public enum TimePeriod
    {
        /// <summary>
        /// Today only
        /// </summary>
        [Display(Name = "Today")]
        Today = 1,

        /// <summary>
        /// This week
        /// </summary>
        [Display(Name = "This Week")]
        ThisWeek = 2,

        /// <summary>
        /// This month
        /// </summary>
        [Display(Name = "This Month")]
        ThisMonth = 3,

        /// <summary>
        /// This quarter
        /// </summary>
        [Display(Name = "This Quarter")]
        ThisQuarter = 4,

        /// <summary>
        /// This year
        /// </summary>
        [Display(Name = "This Year")]
        ThisYear = 5,

        /// <summary>
        /// Last 7 days
        /// </summary>
        [Display(Name = "Last 7 Days")]
        Last7Days = 6,

        /// <summary>
        /// Last 30 days
        /// </summary>
        [Display(Name = "Last 30 Days")]
        Last30Days = 7,

        /// <summary>
        /// Last 90 days
        /// </summary>
        [Display(Name = "Last 90 Days")]
        Last90Days = 8,

        /// <summary>
        /// Last 12 months
        /// </summary>
        [Display(Name = "Last 12 Months")]
        Last12Months = 9,

        /// <summary>
        /// Custom date range
        /// </summary>
        [Display(Name = "Custom Range")]
        Custom = 10
    }
}
