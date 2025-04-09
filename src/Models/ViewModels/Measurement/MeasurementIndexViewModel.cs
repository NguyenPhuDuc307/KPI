namespace KPISolution.Models.ViewModels.Measurement
{
    /// <summary>
    /// View model for the measurement index page
    /// </summary>
    public class MeasurementIndexViewModel
    {
        /// <summary>
        /// Search parameters
        /// </summary>
        public MeasurementSearchViewModel? SearchViewModel { get; set; }

        /// <summary>
        /// List of measurements matching the criteria
        /// </summary>
        public List<MeasurementViewModel> Measurements { get; set; } = [];

        /// <summary>
        /// Total number of measurements matching the criteria for pagination
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Calculate total number of pages
        /// </summary>
        public int TotalPages => this.SearchViewModel != null ? (this.TotalItems + this.SearchViewModel.PageSize - 1) / this.SearchViewModel.PageSize : 1;
    }
}
