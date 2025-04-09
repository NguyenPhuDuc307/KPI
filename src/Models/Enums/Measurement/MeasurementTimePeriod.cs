namespace KPISolution.Models.Enums.Measurement
{
    /// <summary>
    /// Enum representing different time periods for reporting
    /// </summary>
    public enum MeasurementTimePeriod
    {
        /// <summary>
        /// Daily period
        /// </summary>
        [Display(Name = "Hàng ngày")]
        Daily = 1,

        /// <summary>
        /// Weekly period
        /// </summary>
        [Display(Name = "Hàng tuần")]
        Weekly = 2,

        /// <summary>
        /// Monthly period
        /// </summary>
        [Display(Name = "Hàng tháng")]
        Monthly = 3,

        /// <summary>
        /// Quarterly period
        /// </summary>
        [Display(Name = "Hàng quý")]
        Quarterly = 4,

        /// <summary>
        /// Semi-annual period (6 months)
        /// </summary>
        [Display(Name = "Nửa năm")]
        SemiAnnual = 5,

        /// <summary>
        /// Annual period
        /// </summary>
        [Display(Name = "Hàng năm")]
        Annual = 6,

        /// <summary>
        /// Custom defined period
        /// </summary>
        [Display(Name = "Tùy chỉnh")]
        Custom = 7,

        /// <summary>
        /// Last 24 hours
        /// </summary>
        [Display(Name = "24 giờ qua")]
        Last24Hours = 8,

        /// <summary>
        /// Last 7 days
        /// </summary>
        [Display(Name = "7 ngày qua")]
        Last7Days = 9,

        /// <summary>
        /// Last 30 days
        /// </summary>
        [Display(Name = "30 ngày qua")]
        Last30Days = 10,

        /// <summary>
        /// Last 90 days
        /// </summary>
        [Display(Name = "90 ngày qua")]
        Last90Days = 11,

        /// <summary>
        /// Last 6 months
        /// </summary>
        [Display(Name = "6 tháng qua")]
        Last6Months = 12,

        /// <summary>
        /// Last year
        /// </summary>
        [Display(Name = "Năm vừa qua")]
        LastYear = 13,

        /// <summary>
        /// Year to date
        /// </summary>
        [Display(Name = "Từ đầu năm đến nay")]
        YearToDate = 14,

        /// <summary>
        /// Custom date range
        /// </summary>
        [Display(Name = "Phạm vi tùy chọn")]
        CustomRange = 15
    }
}