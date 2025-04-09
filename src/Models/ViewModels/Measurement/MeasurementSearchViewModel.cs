namespace KPISolution.Models.ViewModels.Measurement
{
    /// <summary>
    /// View model for searching and filtering measurements
    /// </summary>
    public class MeasurementSearchViewModel
    {
        /// <summary>
        /// ID of the indicator to filter by
        /// </summary>
        [Display(Name = "Indicator")]
        public Guid? IndicatorId { get; set; }

        /// <summary>
        /// Name of the indicator to search for
        /// </summary>
        [Display(Name = "Indicator Name")]
        public string? IndicatorName { get; set; }

        /// <summary>
        /// Department ID to filter by
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Start date for the date range filter
        /// </summary>
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// End date for the date range filter
        /// </summary>
        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Current page number for pagination
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Sort property name
        /// </summary>
        public string SortProperty { get; set; } = "MeasurementDate";

        /// <summary>
        /// Sort direction
        /// </summary>
        public string SortDirection { get; set; } = "Desc";
    }
}
