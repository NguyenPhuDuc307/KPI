namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    public abstract class WidgetData
    {
        public string WidgetId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Width { get; set; } = 4; // Default to 4 columns (out of 12 in Bootstrap grid)
        public int Height { get; set; } = 2; // Default height multiplier
    }

    /// <summary>
    /// Represents an individual statistic item in a multi-stat display
    /// </summary>
    public class StatWidgetItem
    {
        /// <summary>
        /// Label for the statistic
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Value of the statistic to display
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Optional unit of measurement (%, $, etc.)
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Icon CSS class to display
        /// </summary>
        public string IconClass { get; set; } = string.Empty;

        /// <summary>
        /// Trend direction: "up", "down", or null/empty for neutral
        /// </summary>
        public string Trend { get; set; } = string.Empty;

        /// <summary>
        /// Text explaining the comparison (e.g., "15% so với tháng trước")
        /// </summary>
        public string ComparisonText { get; set; } = string.Empty;
    }

    public class StatisticsWidgetData : WidgetData
    {
        public StatisticsWidgetData()
        {
            this.DetailedStats = new Dictionary<string, string>();
            this.StatItems = [];
            this.StatType = string.Empty;
            this.ChangeDirection = string.Empty;
            this.ColorClass = string.Empty;
        }

        /// <summary>
        /// Type of statistic being displayed
        /// </summary>
        public string StatType { get; set; } // KPI, CSF, Tasks, etc. = string.Empty;

        /// <summary>
        /// The main statistic count
        /// </summary>
        public string Count { get; set; } = string.Empty;

        /// <summary>
        /// The change in value compared to previous period
        /// </summary>
        public int Change { get; set; } // Positive or negative change

        /// <summary>
        /// Direction of change (up/down/none)
        /// </summary>
        public string ChangeDirection { get; set; } // "up" or "down" = string.Empty;

        /// <summary>
        /// Icon CSS class to show
        /// </summary>
        public string IconClass { get; set; } = string.Empty;

        /// <summary>
        /// CSS color class for the stat
        /// </summary>
        public string ColorClass { get; set; } // Bootstrap color class: primary, success, danger, etc. = string.Empty;

        /// <summary>
        /// Additional detailed statistics to display
        /// </summary>
        public Dictionary<string, string> DetailedStats { get; set; }

        /// <summary>
        /// Collection of stat items for multi-stat displays
        /// </summary>
        public List<StatWidgetItem> StatItems { get; set; }

        /// <summary>
        /// Optional footer text to display at the bottom of the widget
        /// </summary>
        public string FooterText { get; set; } = string.Empty;
    }

    public class AlertWidgetData : WidgetData
    {
        public string AlertType { get; set; } = string.Empty; // success, info, warning, danger
        public string Message { get; set; } = string.Empty;
        public string DetailedMessage { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public bool Dismissible { get; set; } = true;
        public bool IsDismissible { get; set; } = true; // Alias for Dismissible
        public string ActionUrl { get; set; } = string.Empty;
        public string ActionText { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; } // Date/time when the alert expires
    }

    public class CalendarWidgetData : WidgetData
    {
        public CalendarWidgetData()
        {
            this.Events = [];
        }

        public DateTime? CurrentMonth { get; set; }
        public List<CalendarEvent> Events { get; set; }
    }

    public class CalendarEvent
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public bool AllDay { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty; // CSS color or Bootstrap class
        public string TextColor { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty; // Type of entity: KPI, CSF, etc.
        public string BorderColor { get; set; } = string.Empty; // Border color for the event

        // Alias properties to maintain compatibility with _CalendarWidget.cshtml
        public DateTime StartDate => this.Start;
        public DateTime? EndDate => this.End;
        public bool IsAllDay => this.AllDay;
        public string Type { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
    }

    public class TableWidgetData : WidgetData
    {
        public List<string> Headers { get; set; } = [];
        public List<TableRow> Rows { get; set; } = [];
        public bool ShowPagination { get; set; }
        public int PageSize { get; set; } = 5;
        public string EmptyMessage { get; set; } = "Không có dữ liệu";
        public string DetailLinkFormat { get; set; } = string.Empty; // Format string for detail links, e.g., "/KPI/Details/{0}"
    }

    public class TableRow
    {
        public string Id { get; set; } = string.Empty;
        public List<TableCell> Cells { get; set; } = [];
        public string ColorClass { get; set; } = string.Empty;
    }

    public class TableCell
    {
        public string Type { get; set; } = string.Empty; // badge, icon, progress, date, html, or default
        public string Value { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
        public int Progress { get; set; } // Used for progress bars
    }
}
