namespace KPISolution.Models.ViewModels.Organization
{
    /// <summary>
    /// ViewModel hiển thị danh sách mục tiêu có phân trang và lọc
    /// </summary>
    public class ObjectiveListViewModel
    {
        [Display(Name = "Tìm kiếm")]
        public string? SearchTerm { get; init; }

        [Display(Name = "Khung thời gian")]
        public TimeframeType? SelectedTimeframe { get; init; }

        [Display(Name = "Khía cạnh kinh doanh")]
        public BusinessPerspective? SelectedPerspective { get; init; }

        [Display(Name = "Trạng thái")]
        public ObjectiveStatus? SelectedStatus { get; init; }

        [Display(Name = "Đơn vị")]
        public Guid? SelectedDepartmentId { get; init; }

        [Display(Name = "Năm")]
        public int? SelectedYear { get; init; }

        [Display(Name = "Chỉ hiển thị mục tiêu cấp cao nhất")]
        public bool ShowOnlyTopLevel { get; init; }

        // Danh sách các mục tiêu
        public List<ObjectiveListItemViewModel> Objectives { get; set; } = [];

        // Thông tin phân trang
        public int CurrentPage { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling(this.TotalItems / (double)this.PageSize);
    }

    /// <summary>
    /// ViewModel hiển thị thông tin của một mục tiêu trong danh sách
    /// </summary>
    public class ObjectiveListItemViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Mã")]
        public string? Code { get; set; }

        [Display(Name = "Tên mục tiêu")]
        public string? Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; } = string.Empty;

        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; }

        [Display(Name = "Trạng thái")]
        public ObjectiveStatus Status { get; set; }

        [Display(Name = "Khung thời gian")]
        public TimeframeType TimeframeType { get; set; }

        [Display(Name = "Khía cạnh")]
        public BusinessPerspective Perspective { get; set; }

        [Display(Name = "Đơn vị")]
        public string? Department { get; set; } = string.Empty;

        [Display(Name = "Người phụ trách")]
        public string? ResponsiblePerson { get; set; } = string.Empty;

        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Có mục tiêu con")]
        public bool HasChildren { get; set; }

        [Display(Name = "Số lượng chỉ số")]
        public int IndicatorCount { get; set; }
    }
}
