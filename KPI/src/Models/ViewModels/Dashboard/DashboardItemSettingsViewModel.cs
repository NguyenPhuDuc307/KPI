using System;
using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.ViewModels.Dashboard
{
    /// <summary>
    /// View model for dashboard item settings
    /// </summary>
    public class DashboardItemSettingsViewModel
    {
        /// <summary>
        /// Gets or sets the item identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the item title
        /// </summary>
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the position (format: "x,y")
        /// </summary>
        [Display(Name = "Position")]
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the size (format: "width,height")
        /// </summary>
        [Display(Name = "Size")]
        public string Size { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the chart type (e.g., "LineChart", "BarChart", etc.)
        /// </summary>
        [Display(Name = "Chart Type")]
        public string ChartType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the time period for data display (e.g., "LastWeek", "LastMonth", etc.)
        /// </summary>
        [Display(Name = "Time Period")]
        public string TimePeriod { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the item type (e.g., "Chart", "Table", "Gauge", etc.)
        /// </summary>
        [Display(Name = "Item Type")]
        public string ItemType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional settings as a JSON string
        /// </summary>
        [Display(Name = "Additional Settings")]
        public string AdditionalSettings { get; set; } = string.Empty;
    }
}