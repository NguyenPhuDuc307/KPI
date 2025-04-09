namespace KPISolution.Models.Entities.Dashboard
{
    /// <summary>
    /// Entity representing an item on a custom dashboard
    /// </summary>
    public class DashboardItem : BaseEntity
    {
        /// <summary>
        /// ID of the dashboard this item belongs to
        /// </summary>
        [Required]
        public Guid DashboardId { get; set; }

        /// <summary>
        /// Reference to the parent dashboard
        /// </summary>
        public virtual CustomDashboard Dashboard { get; set; } = null!;

        /// <summary>
        /// Indicator ID if this item displays a Indicator
        /// </summary>
        public Guid? IndicatorId { get; set; }

        /// <summary>
        /// SuccessFactor ID if this item displays a Success Factor
        /// </summary>
        public Guid? SuccessFactorId { get; set; }

        /// <summary>
        /// Chart type for displaying this item
        /// </summary>
        public ChartType ChartType { get; set; } = ChartType.Card;

        /// <summary>
        /// Title of the item
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Width of the item (1-12 column grid)
        /// </summary>
        [Range(1, 12)]
        public int Width { get; set; } = 4;

        /// <summary>
        /// Height of the item (1-12 row grid)
        /// </summary>
        [Range(1, 12)]
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
        public bool ShowLegend { get; set; } = true;

        /// <summary>
        /// Time period to display
        /// </summary>
        public DisplayTimePeriod TimePeriod { get; set; } = DisplayTimePeriod.Last30Days;
    }
}
