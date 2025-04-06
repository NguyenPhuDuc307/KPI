namespace KPISolution.Models.ViewModels.Organization
{
    /// <summary>
    /// ViewModel hiển thị thông tin về kế hoạch chiến lược
    /// </summary>
    public class StrategicPlanViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Tiêu đề")]
        public string? Title { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Năm bắt đầu")]
        public int StartYear { get; set; }

        [Display(Name = "Năm kết thúc")]
        public int EndYear { get; set; }

        [Display(Name = "Tiến độ tổng thể (%)")]
        public int OverallProgress { get; set; }

        [Display(Name = "Chiến lược")]
        public Guid StrategyId { get; set; }

        [Display(Name = "Tên chiến lược")]
        public string? StrategyName { get; set; }

        [Display(Name = "Người phê duyệt")]
        public string? ApprovedBy { get; set; }

        [Display(Name = "Ngày phê duyệt")]
        [DataType(DataType.Date)]
        public DateTime? ApprovalDate { get; set; }

        [Display(Name = "Các giai đoạn")]
        public List<StrategicPlanPhaseViewModel> Phases { get; set; } = [];

        [Display(Name = "Các mục tiêu chiến lược")]
        public List<StrategicPlanObjectiveViewModel> StrategicObjectives { get; set; } = [];
    }

    /// <summary>
    /// ViewModel hiển thị thông tin về giai đoạn trong kế hoạch chiến lược
    /// </summary>
    public class StrategicPlanPhaseViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Tên giai đoạn")]
        public string? Name { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; }

        [Display(Name = "Trạng thái")]
        public string? Status { get; set; }

        [Display(Name = "Mục tiêu của giai đoạn")]
        public List<PlanPhaseMilestoneViewModel> Milestones { get; set; } = [];
    }

    /// <summary>
    /// ViewModel hiển thị thông tin về mục tiêu quan trọng trong giai đoạn
    /// </summary>
    public class PlanPhaseMilestoneViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Tên mục tiêu")]
        public string? Title { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Ngày hoàn thành dự kiến")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Display(Name = "Đã hoàn thành")]
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// ViewModel hiển thị thông tin về mục tiêu chiến lược trong kế hoạch
    /// </summary>
    public class StrategicPlanObjectiveViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Mã mục tiêu")]
        public string? Code { get; set; }

        [Display(Name = "Tên mục tiêu")]
        public string? Name { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; }

        [Display(Name = "Trạng thái")]
        public ObjectiveStatus Status { get; set; }

        [Display(Name = "Khía cạnh kinh doanh")]
        public BusinessPerspective Perspective { get; set; }

        [Display(Name = "Đơn vị chịu trách nhiệm")]
        public string? Department { get; set; }

        [Display(Name = "Người phụ trách")]
        public string? ResponsiblePerson { get; set; }

        [Display(Name = "Chỉ số đo lường")]
        public List<ObjectiveIndicatorViewModel> Indicators { get; set; } = [];

        [Display(Name = "Yếu tố thành công chính")]
        public List<ObjectiveSuccessFactorViewModel> SuccessFactors { get; set; } = [];
    }
}
