namespace KPISolution.Models.ViewModels.Organization
{
    /// <summary>
    /// ViewModel hiển thị thông tin về chiến lược
    /// </summary>
    public class StrategyViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Mã chiến lược")]
        public string? Code { get; set; }

        [Display(Name = "Tên chiến lược")]
        public string? Name { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Tầm nhìn")]
        public string? Vision { get; set; }

        [Display(Name = "Sứ mệnh")]
        public string? Mission { get; set; }

        [Display(Name = "Giá trị cốt lõi")]
        public string? CoreValues { get; set; }

        [Display(Name = "Năm bắt đầu")]
        public int StartYear { get; set; }

        [Display(Name = "Năm kết thúc")]
        public int EndYear { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; }

        [Display(Name = "Tiến độ tổng thể (%)")]
        public int OverallProgress { get; set; }

        [Display(Name = "Người tạo")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Ngày tạo")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Mục tiêu chiến lược")]
        public List<StrategicObjectiveViewModel> StrategicObjectives { get; set; } = [];
    }

    /// <summary>
    /// ViewModel hiển thị thông tin về mục tiêu chiến lược
    /// </summary>
    public class StrategicObjectiveViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Mã mục tiêu")]
        public string? Code { get; set; }

        [Display(Name = "Tên mục tiêu")]
        public string? Name { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Khía cạnh kinh doanh")]
        public string? Perspective { get; set; }

        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; }

        [Display(Name = "Yếu tố thành công")]
        public int SuccessFactorCount { get; set; }

        [Display(Name = "Chỉ số")]
        public int IndicatorCount { get; set; }
    }
}
