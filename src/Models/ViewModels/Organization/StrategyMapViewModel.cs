namespace KPISolution.Models.ViewModels.Organization
{
    /// <summary>
    /// ViewModel hiển thị bản đồ chiến lược
    /// </summary>
    public class StrategyMapViewModel
    {
        public Guid StrategyId { get; set; }

        [Display(Name = "Tên chiến lược")]
        public string? StrategyName { get; set; }

        [Display(Name = "Năm")]
        public int? SelectedYear { get; set; }

        [Display(Name = "Tầm nhìn")]
        public string? Vision { get; set; }

        [Display(Name = "Sứ mệnh")]
        public string? Mission { get; set; }

        // Các khía cạnh kinh doanh
        [Display(Name = "Tài chính")]
        public List<StrategyMapObjectiveViewModel> FinancialObjectives { get; set; } = [];

        [Display(Name = "Khách hàng")]
        public List<StrategyMapObjectiveViewModel> CustomerObjectives { get; set; } = [];

        [Display(Name = "Quy trình nội bộ")]
        public List<StrategyMapObjectiveViewModel> ProcessObjectives { get; set; } = [];

        [Display(Name = "Học hỏi & phát triển")]
        public List<StrategyMapObjectiveViewModel> LearningObjectives { get; set; } = [];

        // Các mối quan hệ giữa các mục tiêu
        public List<StrategyMapRelationViewModel> Relations { get; set; } = [];
    }

    /// <summary>
    /// ViewModel hiển thị thông tin về mục tiêu trong bản đồ chiến lược
    /// </summary>
    public class StrategyMapObjectiveViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Mã")]
        public string? Code { get; set; }

        [Display(Name = "Tên mục tiêu")]
        public string? Name { get; set; }

        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; }

        [Display(Name = "Số lượng chỉ số")]
        public int IndicatorCount { get; set; }

        // Vị trí hiển thị trên bản đồ
        public int Position { get; set; }
    }

    /// <summary>
    /// ViewModel hiển thị mối quan hệ giữa các mục tiêu trong bản đồ chiến lược
    /// </summary>
    public class StrategyMapRelationViewModel
    {
        public Guid SourceObjectiveId { get; set; }
        public Guid TargetObjectiveId { get; set; }
        public string? RelationType { get; set; } // Direct, Indirect
    }
}
