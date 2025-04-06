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
        public Guid DashboardId { get; init; }

        /// <summary>
        /// Reference to the parent dashboard
        /// </summary>
        public virtual CustomDashboard Dashboard { get; init; } = null!;

        /// <summary>
        /// Indicator ID if this item displays a Indicator
        /// </summary>
        public Guid? IndicatorId { get; init; }

        /// <summary>
        /// SuccessFactor ID if this item displays a Success Factor
        /// </summary>
        public Guid? SuccessFactorId { get; init; }

        /// <summary>
        /// Chart type for displaying this item
        /// </summary>
        public ChartType ChartType { get; init; } = ChartType.Card;

        /// <summary>
        /// Title of the item
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Title { get; init; } = string.Empty;

        /// <summary>
        /// Width of the item (1-12 column grid)
        /// </summary>
        [Range(1, 12)]
        public int Width { get; init; } = 4;

        /// <summary>
        /// Height of the item (1-12 row grid)
        /// </summary>
        [Range(1, 12)]
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
        public bool ShowLegend { get; init; } = true;

        /// <summary>
        /// Time period to display
        /// </summary>
        public DisplayTimePeriod TimePeriod { get; init; } = DisplayTimePeriod.Last30Days;
    }
}
