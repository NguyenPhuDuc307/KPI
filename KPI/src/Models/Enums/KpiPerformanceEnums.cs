using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Represents the trend direction of a KPI's value over time
    /// </summary>
    public enum TrendDirection
    {
        /// <summary>
        /// Value is trending upward
        /// </summary>
        [Display(Name = "Up")]
        Up = 1,

        /// <summary>
        /// Value is trending downward
        /// </summary>
        [Display(Name = "Down")]
        Down = 2,

        /// <summary>
        /// Value is stable or has minimal change
        /// </summary>
        [Display(Name = "Stable")]
        Stable = 3,

        /// <summary>
        /// No trend data is available
        /// </summary>
        [Display(Name = "Not Available")]
        NotAvailable = 4
    }

    /// <summary>
    /// Represents the performance level of a KPI based on its target and actual value
    /// </summary>
    public enum PerformanceLevel
    {
        /// <summary>
        /// Performance is excellent, significantly exceeding target
        /// </summary>
        [Display(Name = "Excellent")]
        Excellent = 1,

        /// <summary>
        /// Performance is good, exceeding target
        /// </summary>
        [Display(Name = "Good")]
        Good = 2,

        /// <summary>
        /// Performance is satisfactory, meeting target
        /// </summary>
        [Display(Name = "Satisfactory")]
        Satisfactory = 3,

        /// <summary>
        /// Performance is below target but within acceptable range
        /// </summary>
        [Display(Name = "Needs Improvement")]
        NeedsImprovement = 4,

        /// <summary>
        /// Performance is poor, significantly below target
        /// </summary>
        [Display(Name = "Poor")]
        Poor = 5,

        /// <summary>
        /// Performance cannot be determined
        /// </summary>
        [Display(Name = "Not Available")]
        NotAvailable = 6
    }
}
