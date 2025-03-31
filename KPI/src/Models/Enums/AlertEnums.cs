using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Represents the severity level of an alert
    /// </summary>
    public enum AlertSeverity
    {
        /// <summary>
        /// Informational alerts that don't require immediate action
        /// </summary>
        [Display(Name = "Info")]
        Info = 1,

        /// <summary>
        /// Warning alerts that may require attention soon
        /// </summary>
        [Display(Name = "Warning")]
        Warning = 2,

        /// <summary>
        /// Critical alerts that require immediate attention
        /// </summary>
        [Display(Name = "Critical")]
        Critical = 3
    }
}