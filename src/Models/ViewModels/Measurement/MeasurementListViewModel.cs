using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Models.ViewModels.Measurement
{
    /// <summary>
    /// ViewModel hiển thị danh sách các phép đo
    /// </summary>
    public class MeasurementListViewModel
    {
        /// <summary>
        /// Bộ lọc hiện tại
        /// </summary>
        public IndicatorMeasurementFilterViewModel Filter { get; set; }

        /// <summary>
        /// Danh sách các phép đo
        /// </summary>
        public List<IndicatorValueViewModel> Items { get; set; } = [];

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Số bản ghi mỗi trang
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(this.TotalCount / (double)this.PageSize);

        /// <summary>
        /// Có trang trước không
        /// </summary>
        public bool HasPreviousPage => this.CurrentPage > 1;

        /// <summary>
        /// Có trang sau không
        /// </summary>
        public bool HasNextPage => this.CurrentPage < this.TotalPages;

        /// <summary>
        /// Danh sách các loại Chỉ số (Performance, Result, Success Factor)
        /// </summary>
        public List<SelectListItem> IndicatorTypes { get; set; } = [];

        /// <summary>
        /// Danh sách các phòng ban
        /// </summary>
        public List<SelectListItem> Departments { get; set; } = [];

        /// <summary>
        /// Danh sách các tần suất đo
        /// </summary>
        public List<SelectListItem> MeasurementFrequencies { get; set; } = [];

        /// <summary>
        /// Constructor
        /// </summary>
        public MeasurementListViewModel()
        {
            this.Filter = new IndicatorMeasurementFilterViewModel();
            this.IndicatorTypes = [];
            this.Departments = [];
            this.MeasurementFrequencies = [];
        }
    }
}
