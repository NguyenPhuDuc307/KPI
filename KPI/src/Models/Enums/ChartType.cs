using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Type of chart that can be used to display data on a dashboard
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// Simple card showing current value
        /// </summary>
        [Display(Name = "Card")]
        Card = 1,

        /// <summary>
        /// Line chart showing trends over time
        /// </summary>
        [Display(Name = "Line Chart")]
        LineChart = 2,

        /// <summary>
        /// Bar chart comparing values
        /// </summary>
        [Display(Name = "Bar Chart")]
        BarChart = 3,

        /// <summary>
        /// Pie chart showing distribution
        /// </summary>
        [Display(Name = "Pie Chart")]
        PieChart = 4,

        /// <summary>
        /// Gauge showing progress against target
        /// </summary>
        [Display(Name = "Gauge")]
        Gauge = 5,

        /// <summary>
        /// Heat map showing intensity
        /// </summary>
        [Display(Name = "Heat Map")]
        HeatMap = 6,

        /// <summary>
        /// Table showing detailed data
        /// </summary>
        [Display(Name = "Data Table")]
        DataTable = 7
    }
}