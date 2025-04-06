using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Models.ViewModels.SuccessFactor
{
    /// <summary>
    /// View model cho việc hiển thị và lọc danh sách yếu tố thành công
    /// </summary>
    public class SuccessFactorListViewModel
    {
        /// <summary>
        /// Constructor initializes the collections and default values
        /// </summary>
        public SuccessFactorListViewModel()
        {
            this.Items = [];
            this.StatusFilter = [];
            this.CategoryFilter = [];
            this.RiskLevelFilter = [];
            this.Departments = new SelectList(new List<SelectListItem>());
            this.BusinessObjectives = new SelectList(new List<SelectListItem>());
            this.SortOptions =
            [
                "Name",
                "Code",
                "Status",
                "Category",
                "TargetDate",
                "Progress",
                "Priority",
                "Risk"
            ];
        }

        /// <summary>
        /// Collection of Success Factor list items to display
        /// </summary>
        public List<SuccessFactorListItemViewModel> Items { get; init; }

        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        [Display(Name = "Tìm kiếm")]
        public string? SearchTerm { get; init; }

        /// <summary>
        /// Loại yếu tố (thông thường hoặc yếu tố cốt lõi)
        /// </summary>
        [Display(Name = "Loại yếu tố")]
        public bool? IsCritical { get; init; }

        /// <summary>
        /// Lọc theo trạng thái
        /// </summary>
        [Display(Name = "Trạng thái")]
        public SuccessFactorStatus? SelectedStatus { get; init; }

        /// <summary>
        /// Lọc theo mức độ rủi ro
        /// </summary>
        [Display(Name = "Mức độ rủi ro")]
        public RiskLevel? SelectedRiskLevel { get; init; }

        /// <summary>
        /// Lọc theo phòng ban
        /// </summary>
        [Display(Name = "Phòng ban")]
        public Guid? SelectedDepartmentId { get; init; }

        /// <summary>
        /// Lọc theo mục tiêu
        /// </summary>
        [Display(Name = "Mục tiêu")]
        public Guid? SelectedObjectiveId { get; init; }

        /// <summary>
        /// Lọc theo danh mục
        /// </summary>
        [Display(Name = "Danh mục")]
        public SuccessFactorCategory SelectedCategory { get; set; }

        /// <summary>
        /// Status filter selections
        /// </summary>
        [Display(Name = "Status")]
        public List<SuccessFactorStatus> StatusFilter { get; init; }

        /// <summary>
        /// Risk level filter selections
        /// </summary>
        [Display(Name = "Risk Level")]
        public List<RiskLevel> RiskLevelFilter { get; init; }

        /// <summary>
        /// Department ID filter
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; init; }

        /// <summary>
        /// Business objective ID filter
        /// </summary>
        [Display(Name = "Business Objective")]
        public Guid? BusinessObjectiveId { get; init; }

        /// <summary>
        /// Start date range filter
        /// </summary>
        [Display(Name = "Start Date From")]
        [DataType(DataType.Date)]
        public DateTime? StartDateFrom { get; init; }

        /// <summary>
        /// Start date range filter
        /// </summary>
        [Display(Name = "Start Date To")]
        [DataType(DataType.Date)]
        public DateTime? StartDateTo { get; init; }

        /// <summary>
        /// Target date range filter
        /// </summary>
        [Display(Name = "Target Date From")]
        [DataType(DataType.Date)]
        public DateTime? TargetDateFrom { get; init; }

        /// <summary>
        /// Target date range filter
        /// </summary>
        [Display(Name = "Target Date To")]
        [DataType(DataType.Date)]
        public DateTime? TargetDateTo { get; init; }

        /// <summary>
        /// Progress percentage minimum filter
        /// </summary>
        [Display(Name = "Min Progress %")]
        [Range(0, 100)]
        public int? MinProgress { get; init; }

        /// <summary>
        /// Progress percentage maximum filter
        /// </summary>
        [Display(Name = "Max Progress %")]
        [Range(0, 100)]
        public int? MaxProgress { get; init; }

        /// <summary>
        /// Whether to show only items needing attention
        /// </summary>
        [Display(Name = "Needs Attention")]
        public bool? NeedsAttention { get; init; }

        /// <summary>
        /// Whether to show only items at risk
        /// </summary>
        [Display(Name = "At Risk")]
        public bool? AtRisk { get; init; }

        /// <summary>
        /// Field to sort by
        /// </summary>
        [Display(Name = "Sort By")]
        public string SortBy { get; init; } = "Name";

        /// <summary>
        /// Sort direction (ascending/descending)
        /// </summary>
        [Display(Name = "Sort Direction")]
        public string SortDirection { get; init; } = "Ascending";

        /// <summary>
        /// Available sort options
        /// </summary>
        public List<string> SortOptions { get; init; }

        /// <summary>
        /// Current page number
        /// </summary>
        public int CurrentPage { get; init; } = 1;

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; init; } = 10;

        /// <summary>
        /// Total count of items
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// Total pages
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(this.TotalCount / (double)this.PageSize);

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPreviousPage => this.CurrentPage > 1;

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNextPage => this.CurrentPage < this.TotalPages;

        /// <summary>
        /// Available departments for filter dropdown
        /// </summary>
        public SelectList Departments { get; init; }

        /// <summary>
        /// Available business objectives for filter dropdown
        /// </summary>
        public SelectList BusinessObjectives { get; init; }

        /// <summary>
        /// Gets or sets the objective ID filter.
        /// </summary>
        [Display(Name = "Objective")]
        public Guid? ObjectiveId { get; init; }

        /// <summary>
        /// Gets or sets the category filter.
        /// </summary>
        [Display(Name = "Category")]
        public List<SuccessFactorCategory>? CategoryFilter { get; set; }

        /// <summary>
        /// Gets or sets the critical filter.
        /// </summary>
        [Display(Name = "Critical Only")]
        public bool? IsCriticalFilter { get; init; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int PageNumber { get; init; } = 1;

        /// <summary>
        /// Gets or sets the total number of records.
        /// </summary>
        public int TotalRecords { get; init; }

        /// <summary>
        /// Gets or sets the total number of items for pagination.
        /// </summary>
        public int TotalItems { get; init; }

        /// <summary>
        /// Gets or sets the list of success factors.
        /// </summary>
        public List<SuccessFactorListItemViewModel> SuccessFactors { get; init; } = [];

        /// <summary>
        /// Gets or sets the list of departments for filtering.
        /// </summary>
        public IEnumerable<SelectListItem> DepartmentList { get; init; } = new List<SelectListItem>();

        /// <summary>
        /// Gets or sets the list of objectives for filtering.
        /// </summary>
        public IEnumerable<SelectListItem> ObjectiveList { get; init; } = new List<SelectListItem>();

        /// <summary>
        /// Gets or sets the list of categories for filtering.
        /// </summary>
        public IEnumerable<SelectListItem> CategoryList { get; init; } = new List<SelectListItem>();
    }
}
