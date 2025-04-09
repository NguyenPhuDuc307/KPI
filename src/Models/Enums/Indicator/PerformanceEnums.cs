namespace KPISolution.Models.Enums.Indicator
{
    /// <summary>
    /// Indicates the trend of a performance indicator's measurements
    /// </summary>
    public enum PerformanceTrend
    {
        [Display(Name = "Chưa thiết lập")]
        NotSet = 0,

        [Display(Name = "Đang cải thiện")]
        Improving = 1,

        [Display(Name = "Ổn định")]
        Stable = 2,

        [Display(Name = "Đang suy giảm")]
        Declining = 3,

        [Display(Name = "Dao động")]
        Fluctuating = 4
    }

    /// <summary>
    /// Represents the performance level of an indicator based on its target and actual value
    /// </summary>
    public enum PerformanceLevel
    {
        /// <summary>
        /// Performance is excellent, significantly exceeding target
        /// </summary>
        [Display(Name = "Xuất sắc")]
        Excellent = 1,

        /// <summary>
        /// Performance is good, exceeding target
        /// </summary>
        [Display(Name = "Tốt")]
        Good = 2,

        /// <summary>
        /// Performance is satisfactory, meeting target
        /// </summary>
        [Display(Name = "Đạt yêu cầu")]
        Satisfactory = 3,

        /// <summary>
        /// Performance is below target but within acceptable range
        /// </summary>
        [Display(Name = "Cần cải thiện")]
        NeedsImprovement = 4,

        /// <summary>
        /// Performance is poor, significantly below target
        /// </summary>
        [Display(Name = "Kém")]
        Poor = 5,

        /// <summary>
        /// Performance cannot be determined
        /// </summary>
        [Display(Name = "Không có dữ liệu")]
        NotAvailable = 6
    }
}