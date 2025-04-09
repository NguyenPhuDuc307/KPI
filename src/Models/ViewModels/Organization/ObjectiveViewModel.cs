namespace KPISolution.Models.ViewModels.Organization
{
    /// <summary>
    /// ViewModel hiển thị thông tin chi tiết về một mục tiêu
    /// </summary>
    public class ObjectiveViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Mã mục tiêu")]
        public string? Code { get; set; }

        [Display(Name = "Tên mục tiêu")]
        public string? Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Trạng thái")]
        public ObjectiveStatus Status { get; set; }

        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; }

        [Display(Name = "Khung thời gian")]
        public TimeframeType Timeframe { get; set; }

        [Display(Name = "Khía cạnh kinh doanh")]
        public BusinessPerspective Perspective { get; set; }

        [Display(Name = "Đơn vị")]
        public string? Department { get; set; } = string.Empty;

        [Display(Name = "Người phụ trách")]
        public string? ResponsiblePerson { get; set; } = string.Empty;

        [Display(Name = "Mục tiêu cha")]
        public Guid? ParentObjectiveId { get; set; }

        [Display(Name = "Mục tiêu cha")]
        public string? ParentObjectiveName { get; set; } = string.Empty;

        [Display(Name = "SuccessFactor")]
        public List<ObjectiveSuccessFactorViewModel> SuccessFactors { get; set; } = [];

        [Display(Name = "Chỉ số đo lường")]
        public List<ObjectiveIndicatorViewModel> Indicators { get; set; } = [];

        [Display(Name = "Mục tiêu con")]
        public List<ObjectiveViewModel> ChildObjectives { get; set; } = [];
    }

    /// <summary>
    /// ViewModel hiển thị thông tin về mục tiêu con
    /// </summary>
    public class ObjectiveSuccessFactorViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Mã")]
        public string? Code { get; set; }

        [Display(Name = "Tên yếu tố")]
        public string? Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; } = string.Empty;

        [Display(Name = "Loại")]
        public string? Type { get; set; } = string.Empty;

        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; }

        [Display(Name = "Trạng thái")]
        public string? Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel hiển thị thông tin về chỉ số thuộc mục tiêu
    /// </summary>
    public class ObjectiveIndicatorViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Mã")]
        public string? Code { get; set; }

        [Display(Name = "Tên chỉ số")]
        public string? Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; } = string.Empty;

        [Display(Name = "Loại")]
        public string? Type { get; set; } = string.Empty;

        [Display(Name = "Giá trị hiện tại")]
        public decimal CurrentValue { get; set; }

        [Display(Name = "Giá trị mục tiêu")]
        public decimal TargetValue { get; set; }

        [Display(Name = "Đơn vị đo")]
        public string? Unit { get; set; } = string.Empty;

        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; }
    }
}
