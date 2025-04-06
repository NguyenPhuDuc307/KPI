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
        [Display(Name = "Daily")]
        Daily = 1,

        /// <summary>
        /// Weekly period
        /// </summary>
        [Display(Name = "Weekly")]
        Weekly = 2,

        /// <summary>
        /// Monthly period
        /// </summary>
        [Display(Name = "Monthly")]
        Monthly = 3,

        /// <summary>
        /// Quarterly period
        /// </summary>
        [Display(Name = "Quarterly")]
        Quarterly = 4,

        /// <summary>
        /// Semi-annual period (6 months)
        /// </summary>
        [Display(Name = "Semi-Annual")]
        SemiAnnual = 5,

        /// <summary>
        /// Annual period
        /// </summary>
        [Display(Name = "Annual")]
        Annual = 6,

        /// <summary>
        /// Custom defined period
        /// </summary>
        [Display(Name = "Custom")]
        Custom = 7,

        /// <summary>
        /// Last 24 hours
        /// </summary>
        [Display(Name = "Last 24 Hours")]
        Last24Hours = 8,

        /// <summary>
        /// Last 7 days
        /// </summary>
        [Display(Name = "Last 7 Days")]
        Last7Days = 9,

        /// <summary>
        /// Last 30 days
        /// </summary>
        [Display(Name = "Last 30 Days")]
        Last30Days = 10,

        /// <summary>
        /// Last 90 days
        /// </summary>
        [Display(Name = "Last 90 Days")]
        Last90Days = 11,

        /// <summary>
        /// Last 6 months
        /// </summary>
        [Display(Name = "Last 6 Months")]
        Last6Months = 12,

        /// <summary>
        /// Last year
        /// </summary>
        [Display(Name = "Last Year")]
        LastYear = 13,

        /// <summary>
        /// Year to date
        /// </summary>
        [Display(Name = "Year to Date")]
        YearToDate = 14,

        /// <summary>
        /// Custom date range
        /// </summary>
        [Display(Name = "Custom Range")]
        CustomRange = 15
    }
}