using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Enum representing different types of KPI
    /// </summary>
    public enum KpiType
    {
        /// <summary>
        /// Key Result Indicator - measures critical success factors
        /// </summary>
        [Display(Name = "Key Result Indicator")]
        KeyResultIndicator = 1,

        /// <summary>
        /// Result Indicator - measures what has been done
        /// </summary>
        [Display(Name = "Result Indicator")]
        ResultIndicator = 2,

        /// <summary>
        /// Performance Indicator - measures what to do to increase performance
        /// </summary>
        [Display(Name = "Performance Indicator")]
        PerformanceIndicator = 3
    }

    /// <summary>
    /// Enum representing the relationship strength between CSF and KPI
    /// </summary>
    public enum RelationshipStrength
    {
        /// <summary>
        /// Weak influence
        /// </summary>
        Weak = 1,

        /// <summary>
        /// Moderate influence
        /// </summary>
        Moderate = 2,

        /// <summary>
        /// Strong influence
        /// </summary>
        Strong = 3,

        /// <summary>
        /// Critical influence
        /// </summary>
        Critical = 4
    }

    /// <summary>
    /// Enum representing different notification types
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Threshold breach notification
        /// </summary>
        ThresholdBreach = 1,

        /// <summary>
        /// Target achievement notification
        /// </summary>
        TargetAchieved = 2,

        /// <summary>
        /// KPI update notification
        /// </summary>
        KpiUpdated = 3,

        /// <summary>
        /// Reminder notification
        /// </summary>
        Reminder = 4,

        /// <summary>
        /// Custom notification
        /// </summary>
        Custom = 5
    }

    /// <summary>
    /// Enum representing different notification priorities
    /// </summary>
    public enum NotificationPriority
    {
        /// <summary>
        /// Low priority notification
        /// </summary>
        Low = 1,

        /// <summary>
        /// Medium priority notification
        /// </summary>
        Medium = 2,

        /// <summary>
        /// High priority notification
        /// </summary>
        High = 3,

        /// <summary>
        /// Urgent priority notification
        /// </summary>
        Urgent = 4
    }

    /// <summary>
    /// Enum representing different business perspectives based on Balanced Scorecard
    /// </summary>
    public enum BusinessPerspective
    {
        /// <summary>
        /// Financial perspective
        /// </summary>
        Financial = 1,

        /// <summary>
        /// Customer perspective
        /// </summary>
        Customer = 2,

        /// <summary>
        /// Internal Business Process perspective
        /// </summary>
        InternalProcess = 3,

        /// <summary>
        /// Learning and Growth perspective
        /// </summary>
        LearningGrowth = 4,

        /// <summary>
        /// Other perspectives
        /// </summary>
        Other = 5
    }

    /// <summary>
    /// Enum representing different priority levels
    /// </summary>
    public enum PriorityLevel
    {
        /// <summary>
        /// Low priority
        /// </summary>
        Low = 1,

        /// <summary>
        /// Medium priority
        /// </summary>
        Medium = 2,

        /// <summary>
        /// High priority
        /// </summary>
        High = 3,

        /// <summary>
        /// Critical priority
        /// </summary>
        Critical = 4
    }

    /// <summary>
    /// Enum representing different measurement statuses
    /// </summary>
    public enum MeasurementStatus
    {
        /// <summary>
        /// Measurement has been recorded but not verified
        /// </summary>
        [Display(Name = "Recorded")]
        Recorded = 1,

        /// <summary>
        /// Measurement has been verified and approved
        /// </summary>
        [Display(Name = "Verified")]
        Verified = 2,

        /// <summary>
        /// Measurement has been rejected due to issues
        /// </summary>
        [Display(Name = "Rejected")]
        Rejected = 3,

        /// <summary>
        /// Measurement is pending verification
        /// </summary>
        [Display(Name = "Pending")]
        Pending = 4,

        /// <summary>
        /// Measurement needs to be revised
        /// </summary>
        [Display(Name = "Needs Revision")]
        NeedsRevision = 5
    }
}
