using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.Dashboard
{
    /// <summary>
    /// View model for custom user dashboards allowing personalized KPI monitoring
    /// </summary>
    public class CustomDashboardViewModel
    {
        /// <summary>
        /// Constructor to initialize collections
        /// </summary>
        public CustomDashboardViewModel()
        {
            DashboardItems = new List<DashboardItemViewModel>();
            AvailableKpis = new List<KpiSummaryViewModel>();
            SavedLayouts = new List<DashboardLayoutViewModel>();
            SharedUsers = new List<DashboardUserViewModel>();
            AvailableUsers = new List<DashboardUserViewModel>();
            AvailableCsfs = new List<CsfSummaryViewModel>();
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
        public List<DashboardItemViewModel> DashboardItems { get; set; } = new List<DashboardItemViewModel>();

        /// <summary>
        /// Alias for DashboardItems for backward compatibility
        /// </summary>
        public List<DashboardItemViewModel> Items => DashboardItems;

        /// <summary>
        /// KPIs available to add to the dashboard
        /// </summary>
        [Display(Name = "Available KPIs")]
        public List<KpiSummaryViewModel> AvailableKpis { get; set; } = new List<KpiSummaryViewModel>();

        /// <summary>
        /// CSFs available to add to the dashboard
        /// </summary>
        [Display(Name = "Available CSFs")]
        public List<CsfSummaryViewModel> AvailableCsfs { get; set; } = new List<CsfSummaryViewModel>();

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
        public List<DashboardUserViewModel> SharedUsers { get; set; } = new List<DashboardUserViewModel>();

        /// <summary>
        /// Users available for sharing
        /// </summary>
        [Display(Name = "Available Users")]
        public List<DashboardUserViewModel> AvailableUsers { get; set; } = new List<DashboardUserViewModel>();

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
        /// KPI ID
        /// </summary>
        public Guid? KpiId { get; set; }

        /// <summary>
        /// CSF ID
        /// </summary>
        public Guid? CsfId { get; set; }

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
        /// Item type (KPI, CSF, custom)
        /// </summary>
        public DashboardItemType ItemType { get; set; } = DashboardItemType.Kpi;

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
    /// Type of dashboard item
    /// </summary>
    public enum DashboardItemType
    {
        /// <summary>
        /// KPI chart or card
        /// </summary>
        [Display(Name = "KPI")]
        Kpi = 1,

        /// <summary>
        /// CSF progress
        /// </summary>
        [Display(Name = "CSF")]
        Csf = 2,

        /// <summary>
        /// Department overview
        /// </summary>
        [Display(Name = "Department")]
        Department = 3,

        /// <summary>
        /// Text or information card
        /// </summary>
        [Display(Name = "Text")]
        Text = 4,

        /// <summary>
        /// Custom metric
        /// </summary>
        [Display(Name = "Custom")]
        Custom = 5
    }

    /// <summary>
    /// Types of charts available for dashboard items
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// Card with key metrics
        /// </summary>
        [Display(Name = "Card")]
        Card = 1,

        /// <summary>
        /// Line chart showing trends
        /// </summary>
        [Display(Name = "Line Chart")]
        LineChart = 2,

        /// <summary>
        /// Bar chart for comparisons
        /// </summary>
        [Display(Name = "Bar Chart")]
        BarChart = 3,

        /// <summary>
        /// Pie chart for distribution
        /// </summary>
        [Display(Name = "Pie Chart")]
        PieChart = 4,

        /// <summary>
        /// Gauge chart for progress
        /// </summary>
        [Display(Name = "Gauge")]
        Gauge = 5,

        /// <summary>
        /// Table with detailed data
        /// </summary>
        [Display(Name = "Table")]
        Table = 6
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