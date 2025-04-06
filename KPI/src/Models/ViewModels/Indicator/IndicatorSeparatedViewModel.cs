namespace KPISolution.Models.ViewModels.Indicator
{
    public class IndicatorSeparatedViewModel
    {
        public IndicatorSeparatedViewModel()
        {
            this.SuccessFactors = [];
            this.KeyResultIndicators = [];
            this.ResultIndicators = [];
            this.KeyPerformanceIndicators = [];
            this.PerformanceIndicators = [];
        }

        public List<IndicatorCategoryViewModel> SuccessFactors { get; init; }
        public List<IndicatorItemViewModel> KeyResultIndicators { get; init; }
        public List<IndicatorItemViewModel> ResultIndicators { get; init; }
        public List<IndicatorItemViewModel> KeyPerformanceIndicators { get; init; }
        public List<IndicatorItemViewModel> PerformanceIndicators { get; init; }
    }

    public class IndicatorCategoryViewModel
    {
        public IndicatorCategoryViewModel()
        {
            this.Items = [];
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCritical { get; set; }
        public int Progress { get; set; }
        public string ProgressClass { get; set; } = string.Empty;
        public List<IndicatorItemViewModel> Items { get; set; }
    }

    public class IndicatorItemViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsKey { get; set; }
        public string Type { get; set; } = string.Empty;
        public string TypeClass { get; set; } = string.Empty;
        public string? ActualValue { get; set; }
        public string? TargetValue { get; set; }
        public string? Unit { get; set; }
        public int? Achievement { get; set; }
        public string? Status { get; set; }
        public string? StatusClass { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string? Department { get; set; }
    }
}
