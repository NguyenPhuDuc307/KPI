using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Enum representing different categories of Critical Success Factors
    /// </summary>
    public enum CSFCategory
    {
        /// <summary>
        /// Financial perspective CSFs
        /// </summary>
        [Display(Name = "Financial")]
        Financial = 1,

        /// <summary>
        /// Customer perspective CSFs
        /// </summary>
        [Display(Name = "Customer")]
        Customer = 2,

        /// <summary>
        /// Internal business process perspective CSFs
        /// </summary>
        [Display(Name = "Internal Process")]
        InternalProcess = 3,

        /// <summary>
        /// Learning and growth perspective CSFs
        /// </summary>
        [Display(Name = "Learning and Growth")]
        LearningAndGrowth = 4,

        /// <summary>
        /// Other category not fitting into the balanced scorecard perspectives
        /// </summary>
        Other = 5
    }

    /// <summary>
    /// Enum representing different statuses for a CSF
    /// </summary>
    public enum CSFStatus
    {
        /// <summary>
        /// The CSF has not been started
        /// </summary>
        [Display(Name = "Not Started")]
        NotStarted = 1,

        /// <summary>
        /// Work on the CSF is in progress
        /// </summary>
        [Display(Name = "In Progress")]
        InProgress = 2,

        /// <summary>
        /// The CSF is at risk of not being achieved
        /// </summary>
        AtRisk = 3,

        /// <summary>
        /// The CSF is delayed
        /// </summary>
        Delayed = 4,

        /// <summary>
        /// The CSF has been completed successfully
        /// </summary>
        [Display(Name = "Completed")]
        Completed = 5,

        /// <summary>
        /// The CSF has been cancelled
        /// </summary>
        [Display(Name = "Cancelled")]
        Cancelled = 6
    }

    /// <summary>
    /// Enum representing risk levels associated with a CSF
    /// </summary>
    public enum RiskLevel
    {
        /// <summary>
        /// Low risk level
        /// </summary>
        Low = 1,

        /// <summary>
        /// Medium risk level
        /// </summary>
        Medium = 2,

        /// <summary>
        /// High risk level
        /// </summary>
        High = 3,

        /// <summary>
        /// Critical risk level
        /// </summary>
        Critical = 4
    }
}
