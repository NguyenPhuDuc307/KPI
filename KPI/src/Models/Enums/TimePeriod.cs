using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Represents different time periods for dashboard data display
    /// </summary>
    public enum TimePeriod
    {
        [Display(Name = "Last 24 Hours")]
        Last24Hours = 1,

        [Display(Name = "Last 7 Days")]
        Last7Days = 2,

        [Display(Name = "Last 30 Days")]
        Last30Days = 3,

        [Display(Name = "Last 90 Days")]
        Last90Days = 4,

        [Display(Name = "Last 6 Months")]
        Last6Months = 5,

        [Display(Name = "Last Year")]
        LastYear = 6,

        [Display(Name = "Year to Date")]
        YearToDate = 7,

        [Display(Name = "Custom Range")]
        CustomRange = 8
    }
}