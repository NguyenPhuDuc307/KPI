namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    public abstract class WidgetData
    {
        public string WidgetId { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int Width { get; init; } = 4; // Default to 4 columns (out of 12 in Bootstrap grid)
        public int Height { get; init; } = 2; // Default height multiplier
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
        public string StatType { get; init; } // KPI, CSF, Tasks, etc. = string.Empty;

        /// <summary>
        /// The main statistic count
        /// </summary>
        public string Count { get; init; } = string.Empty;

        /// <summary>
        /// The change in value compared to previous period
        /// </summary>
        public int Change { get; init; } // Positive or negative change

        /// <summary>
        /// Direction of change (up/down/none)
        /// </summary>
        public string ChangeDirection { get; init; } // "up" or "down" = string.Empty;

        /// <summary>
        /// Icon CSS class to show
        /// </summary>
        public string IconClass { get; init; } = string.Empty;

        /// <summary>
        /// CSS color class for the stat
        /// </summary>
        public string ColorClass { get; init; } // Bootstrap color class: primary, success, danger, etc. = string.Empty;

        /// <summary>
        /// Additional detailed statistics to display
        /// </summary>
        public Dictionary<string, string> DetailedStats { get; init; }

        /// <summary>
        /// Collection of stat items for multi-stat displays
        /// </summary>
        public List<StatWidgetItem> StatItems { get; init; }

        /// <summary>
        /// Optional footer text to display at the bottom of the widget
        /// </summary>
        public string FooterText { get; init; } = string.Empty;
    }

    public class AlertWidgetData : WidgetData
    {
        public string AlertType { get; init; } = string.Empty; // success, info, warning, danger
        public string Message { get; init; } = string.Empty;
        public string DetailedMessage { get; init; } = string.Empty;
        public string IconClass { get; init; } = string.Empty;
        public bool Dismissible { get; init; } = true;
        public bool IsDismissible { get; init; } = true; // Alias for Dismissible
        public string ActionUrl { get; init; } = string.Empty;
        public string ActionText { get; init; } = string.Empty;
        public DateTime? ExpiresAt { get; init; } // Date/time when the alert expires
    }

    public class CalendarWidgetData : WidgetData
    {
        public CalendarWidgetData()
        {
            this.Events = [];
        }

        public DateTime? CurrentMonth { get; init; }
        public List<CalendarEvent> Events { get; init; }
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

    public class ChartWidgetData : WidgetData
    {
        public string ChartType { get; init; } = string.Empty; // line, bar, pie, doughnut, etc.
        public List<string> Labels { get; init; } = [];
        public List<ChartDataset> Datasets { get; init; } = [];
        public ChartOptions Options { get; init; } = new ChartOptions();

        // Properties to maintain backward compatibility
        public bool ShowLegend => this.Options?.ShowLegend ?? true;
        public string DataSource { get; init; } = string.Empty;
        public bool StartFromZero { get; init; } = true;
    }

    public class ChartDataset
    {
        public string Label { get; set; } = string.Empty;
        public List<double> Data { get; set; } = [];
        public string BorderColor { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
        public bool Fill { get; set; }
        public int BorderWidth { get; set; } = 1;
    }

    public class ChartOptions
    {
        public bool Responsive { get; set; } = true;
        public bool MaintainAspectRatio { get; set; } = false;
        public int? SuggestedMin { get; set; }
        public int? SuggestedMax { get; set; }
        public bool ShowLegend { get; set; } = true;
        public string LegendPosition { get; set; } = "top";
        public bool StartFromZero { get; set; } = true;
    }

    public class TableWidgetData : WidgetData
    {
        public List<string> Headers { get; init; } = [];
        public List<TableRow> Rows { get; init; } = [];
        public bool ShowPagination { get; init; }
        public int PageSize { get; init; } = 5;
        public string EmptyMessage { get; init; } = "Không có dữ liệu";
        public string DetailLinkFormat { get; init; } = string.Empty; // Format string for detail links, e.g., "/KPI/Details/{0}"
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
