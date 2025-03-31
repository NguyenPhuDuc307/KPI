using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Enum representing different statuses for objectives and CSFs
    /// </summary>
    public enum ObjectiveStatus
    {
        /// <summary>
        /// Not started yet
        /// </summary>
        [Display(Name = "Not Started")]
        NotStarted = 1,

        /// <summary>
        /// In planning stage
        /// </summary>
        [Display(Name = "Planning")]
        Planning = 2,

        /// <summary>
        /// In progress
        /// </summary>
        [Display(Name = "In Progress")]
        InProgress = 3,

        /// <summary>
        /// On hold temporarily
        /// </summary>
        [Display(Name = "On Hold")]
        OnHold = 4,

        /// <summary>
        /// Completed successfully
        /// </summary>
        [Display(Name = "Completed")]
        Completed = 5,

        /// <summary>
        /// Canceled
        /// </summary>
        [Display(Name = "Canceled")]
        Canceled = 6,

        /// <summary>
        /// Delayed
        /// </summary>
        [Display(Name = "Delayed")]
        Delayed = 7,

        /// <summary>
        /// At risk of not meeting target
        /// </summary>
        [Display(Name = "At Risk")]
        AtRisk = 8
    }

    /// <summary>
    /// Enum representing different timeframe types
    /// </summary>
    public enum TimeframeType
    {
        /// <summary>
        /// Short term (less than 1 year)
        /// </summary>
        [Display(Name = "Short-Term")]
        ShortTerm = 1,

        /// <summary>
        /// Medium term (1-3 years)
        /// </summary>
        [Display(Name = "Medium-Term")]
        MediumTerm = 2,

        /// <summary>
        /// Long term (more than 3 years)
        /// </summary>
        [Display(Name = "Long-Term")]
        LongTerm = 3
    }

    // CSFCategory and CSFStatus enums removed because they're already defined elsewhere
}
