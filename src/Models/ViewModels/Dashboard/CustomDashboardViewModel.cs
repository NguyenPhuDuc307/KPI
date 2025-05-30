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
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the dashboard
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        [Display(Name = "Dashboard Title")]
        public required string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description of the dashboard
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// User ID who owns this dashboard
        /// </summary>
        public required string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Username who owns this dashboard
        /// </summary>
        [Display(Name = "Owner")]
        public required string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Date/time when the dashboard was last refreshed
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date/time when the dashboard configuration was last modified
        /// </summary>
        [Display(Name = "Last Modified")]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Items shown on this dashboard
        /// </summary>
        public List<DashboardItemViewModel> DashboardItems { get; set; } = [];

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
        public List<SuccessFactorSummaryViewModel> AvailableSuccessFactors { get; set; } = [];

        /// <summary>
        /// Saved dashboard layouts
        /// </summary>
        [Display(Name = "Saved Layouts")]
        public List<DashboardLayoutViewModel> SavedLayouts { get; set; }

        /// <summary>
        /// Layout configuration (JSON)
        /// </summary>
        public string? LayoutConfiguration { get; set; }

        /// <summary>
        /// Whether this dashboard is the user's default
        /// </summary>
        [Display(Name = "Set as Default")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Whether this dashboard is shared with others
        /// </summary>
        [Display(Name = "Share Dashboard")]
        public bool IsShared { get; set; }

        /// <summary>
        /// Whether this dashboard is public (visible to all users)
        /// </summary>
        [Display(Name = "Public Dashboard")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Users this dashboard is shared with
        /// </summary>
        [Display(Name = "Shared With")]
        public List<DashboardUserViewModel> SharedUsers { get; set; } = [];

        /// <summary>
        /// Users available for sharing
        /// </summary>
        [Display(Name = "Available Users")]
        public List<DashboardUserViewModel> AvailableUsers { get; set; } = [];

        /// <summary>
        /// Refresh interval in minutes
        /// </summary>
        [Display(Name = "Auto-refresh (minutes)")]
        [Range(0, 1440, ErrorMessage = "Refresh interval must be between 0 and 1440 minutes")]
        public int RefreshInterval { get; set; }
    }

    /// <summary>
    /// Individual item on a custom dashboard
    /// </summary>
    public class DashboardItemViewModel
    {
        /// <summary>
        /// Item ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Indicator ID
        /// </summary>
        public Guid? IndicatorId { get; set; }

        /// <summary>
        /// SuccessFactor ID if this item displays a Success Factor
        /// </summary>
        public Guid? SuccessFactorId { get; set; }

        /// <summary>
        /// Chart type for displaying this item
        /// </summary>
        [Display(Name = "Chart Type")]
        public ChartType ChartType { get; set; } = ChartType.Card;

        /// <summary>
        /// Title of the item
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Width of the item (1-12 column grid)
        /// </summary>
        [Range(1, 12)]
        [Display(Name = "Width")]
        public int Width { get; set; } = 4;

        /// <summary>
        /// Height of the item (1-12 row grid)
        /// </summary>
        [Range(1, 12)]
        [Display(Name = "Height")]
        public int Height { get; set; } = 4;

        /// <summary>
        /// X position (column)
        /// </summary>
        [Range(0, 11)]
        public int X { get; set; }

        /// <summary>
        /// Y position (row)
        /// </summary>
        [Range(0, 11)]
        public int Y { get; set; }

        /// <summary>
        /// Data configuration (JSON)
        /// </summary>
        public string DataConfiguration { get; set; } = string.Empty;

        /// <summary>
        /// Order in which the item appears (for non-grid layouts)
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Item type (Indicator, SuccessFactor, custom)
        /// </summary>
        public DashboardItemType ItemType { get; set; } = DashboardItemType.Chart;

        /// <summary>
        /// Whether to show legends on charts
        /// </summary>
        [Display(Name = "Show Legend")]
        public bool ShowLegend { get; set; } = true;

        /// <summary>
        /// Time period to display
        /// </summary>
        [Display(Name = "Time Period")]
        public TimePeriod TimePeriod { get; set; } = TimePeriod.Last30Days;

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
        /// Custom time period
        /// </summary>
        [Display(Name = "Custom")]
        Custom = 0,

        /// <summary>
        /// Today only
        /// </summary>
        [Display(Name = "Today")]
        Today = 1,

        /// <summary>
        /// Yesterday only
        /// </summary>
        [Display(Name = "Yesterday")]
        Yesterday = 2,

        /// <summary>
        /// This week
        /// </summary>
        [Display(Name = "This Week")]
        ThisWeek = 3,

        /// <summary>
        /// Last week
        /// </summary>
        [Display(Name = "Last Week")]
        LastWeek = 4,

        /// <summary>
        /// This month
        /// </summary>
        [Display(Name = "This Month")]
        ThisMonth = 5,

        /// <summary>
        /// Last month
        /// </summary>
        [Display(Name = "Last Month")]
        LastMonth = 6,

        /// <summary>
        /// Last 30 days
        /// </summary>
        [Display(Name = "Last 30 Days")]
        Last30Days = 7,

        /// <summary>
        /// This quarter
        /// </summary>
        [Display(Name = "This Quarter")]
        ThisQuarter = 8,

        /// <summary>
        /// Last quarter
        /// </summary>
        [Display(Name = "Last Quarter")]
        LastQuarter = 9,

        /// <summary>
        /// This year
        /// </summary>
        [Display(Name = "This Year")]
        ThisYear = 10,

        /// <summary>
        /// Last year
        /// </summary>
        [Display(Name = "Last Year")]
        LastYear = 11,

        /// <summary>
        /// Year to date
        /// </summary>
        [Display(Name = "Year to Date")]
        YearToDate = 12
    }
}
