namespace KPISolution.Models.Enums.Measurement
{
    /// <summary>
    /// Defines the type of indicator being measured
    /// </summary>
    public enum IndicatorMeasurementType
    {
        /// <summary>
        /// Unknown or unspecified indicator type
        /// </summary>
        [Display(Name = "Unknown")]
        Unknown = 0,

        /// <summary>
        /// Performance Indicator measurement
        /// </summary>
        [Display(Name = "Performance Indicator")]
        PerformanceIndicator = 1,

        /// <summary>
        /// Result Indicator measurement
        /// </summary>
        [Display(Name = "Result Indicator")]
        ResultIndicator = 2,

        /// <summary>
        /// Success Factor measurement
        /// </summary>
        [Display(Name = "Success Factor")]
        SuccessFactor = 3
    }
}