using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Enum representing indicator hierarchy types
    /// </summary>
    public enum IndicatorHierarchyType
    {
        /// <summary>
        /// Objective - Strategic goals or objectives
        /// </summary>
        [Display(Name = "Objective")]
        Objective = 1,

        /// <summary>
        /// Success Factor - Factors contributing to objectives
        /// </summary>
        [Display(Name = "Success Factor")]
        SuccessFactor = 2,

        /// <summary>
        /// Critical Success Factor - Critical elements for strategy success
        /// </summary>
        [Display(Name = "Critical Success Factor")]
        CriticalSuccessFactor = 3,

        /// <summary>
        /// Result Indicator - Measures what has been achieved
        /// </summary>
        [Display(Name = "Result Indicator")]
        ResultIndicator = 4,

        /// <summary>
        /// Performance Indicator - Measures what to do to improve performance
        /// </summary>
        [Display(Name = "Performance Indicator")]
        PerformanceIndicator = 5,

        /// <summary>
        /// Key Result Indicator - Key measures of what has been achieved
        /// </summary>
        [Display(Name = "Key Result Indicator")]
        KeyResultIndicator = 6,

        /// <summary>
        /// Key Performance Indicator - Key measures of what to do to improve performance
        /// </summary>
        [Display(Name = "Key Performance Indicator")]
        KeyPerformanceIndicator = 7
    }

    /// <summary>
    /// Enum representing the indicator type (Leading vs Lagging)
    /// </summary>
    public enum IndicatorType
    {
        /// <summary>
        /// Leading indicators predict future performance
        /// </summary>
        [Display(Name = "Leading")]
        Leading = 1,

        /// <summary>
        /// Lagging indicators show historical performance
        /// </summary>
        [Display(Name = "Lagging")]
        Lagging = 2,

        /// <summary>
        /// Mixed indicators have both leading and lagging characteristics
        /// </summary>
        [Display(Name = "Mixed")]
        Mixed = 3
    }

    /// <summary>
    /// Enum representing the measurement direction
    /// </summary>
    public enum MeasurementDirection
    {
        /// <summary>
        /// Higher values are better
        /// </summary>
        [Display(Name = "Higher is Better")]
        HigherIsBetter = 1,

        /// <summary>
        /// Lower values are better
        /// </summary>
        [Display(Name = "Lower is Better")]
        LowerIsBetter = 2,

        /// <summary>
        /// Values closer to target are better
        /// </summary>
        [Display(Name = "Closer to Target")]
        CloserToTarget = 3
    }

    /// <summary>
    /// Enum representing the measurement frequency
    /// </summary>
    public enum MeasurementFrequency
    {
        /// <summary>
        /// Measured daily
        /// </summary>
        [Display(Name = "Daily")]
        Daily = 1,

        /// <summary>
        /// Measured weekly
        /// </summary>
        [Display(Name = "Weekly")]
        Weekly = 2,

        /// <summary>
        /// Measured bi-weekly (every two weeks)
        /// </summary>
        [Display(Name = "Bi-Weekly")]
        BiWeekly = 3,

        /// <summary>
        /// Measured monthly
        /// </summary>
        [Display(Name = "Monthly")]
        Monthly = 4,

        /// <summary>
        /// Measured quarterly
        /// </summary>
        [Display(Name = "Quarterly")]
        Quarterly = 5,

        /// <summary>
        /// Measured semi-annually
        /// </summary>
        [Display(Name = "Semi-Annual")]
        SemiAnnual = 6,

        /// <summary>
        /// Measured annually
        /// </summary>
        [Display(Name = "Annual")]
        Annual = 7,

        /// <summary>
        /// Custom measurement frequency
        /// </summary>
        [Display(Name = "Custom")]
        Custom = 8
    }
}