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
        [Display(Name = "Không xác định")]
        Unknown = 0,

        /// <summary>
        /// Performance Indicator measurement
        /// </summary>
        [Display(Name = "Chỉ số hiệu suất")]
        PerformanceIndicator = 1,

        /// <summary>
        /// Result Indicator measurement
        /// </summary>
        [Display(Name = "Chỉ số kết quả")]
        ResultIndicator = 2,

        /// <summary>
        /// Success Factor measurement
        /// </summary>
        [Display(Name = "SuccessFactor")]
        SuccessFactor = 3
    }
}