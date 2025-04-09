namespace KPISolution.Models.ViewModels.Hierarchy
{
    /// <summary>
    /// View model for displaying hierarchical relationship between indicators
    /// </summary>
    public class IndicatorHierarchyViewModel
    {
        // Title and description
        [Display(Name = "Title")]
        public string Title { get; set; } = "Indicator Hierarchy";

        [Display(Name = "Description")]
        public string Description { get; set; } = "Hierarchical view of organizational objectives, success factors, and performance/result indicators";

        // Filter properties
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }
        public IEnumerable<SelectListItem> DepartmentOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Objective")]
        public Guid? ObjectiveId { get; set; }
        public IEnumerable<SelectListItem> ObjectiveOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "View Mode")]
        public string ViewMode { get; set; } = "Full"; // Options: Full, Compact, Summary
        public IEnumerable<SelectListItem> ViewModeOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Status Filter")]
        public string? StatusFilter { get; set; }
        public IEnumerable<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();

        // Collections - Hierarchy items
        public ICollection<ObjectiveListItemViewModel> Objectives { get; set; } = new List<ObjectiveListItemViewModel>();
        public ICollection<SuccessFactorListItemViewModel> SuccessFactors { get; set; } = new List<SuccessFactorListItemViewModel>();
        public ICollection<ResultIndicatorListItemViewModel> ResultIndicators { get; set; } = new List<ResultIndicatorListItemViewModel>();
        public ICollection<PerformanceIndicatorListItemViewModel> PerformanceIndicators { get; set; } = new List<PerformanceIndicatorListItemViewModel>();

        // Extension properties for SuccessFactorListItemViewModel to add relationships with indicators
        // Sẽ được thiết lập trong controller
        public Dictionary<Guid, List<ResultIndicatorListItemViewModel>> SuccessFactorResultIndicators { get; set; } =
            new Dictionary<Guid, List<ResultIndicatorListItemViewModel>>();

        public Dictionary<Guid, List<PerformanceIndicatorListItemViewModel>> SuccessFactorPerformanceIndicators { get; set; } =
            new Dictionary<Guid, List<PerformanceIndicatorListItemViewModel>>();

        // Helper methods
        public IEnumerable<ResultIndicatorListItemViewModel> GetResultIndicatorsForSuccessFactor(SuccessFactorListItemViewModel successFactor)
        {
            if (this.SuccessFactorResultIndicators.TryGetValue(successFactor.Id, out var indicators))
            {
                return indicators;
            }
            return [];
        }

        public IEnumerable<PerformanceIndicatorListItemViewModel> GetPerformanceIndicatorsForSuccessFactor(SuccessFactorListItemViewModel successFactor)
        {
            if (this.SuccessFactorPerformanceIndicators.TryGetValue(successFactor.Id, out var indicators))
            {
                return indicators;
            }
            return [];
        }

        public List<ResultIndicatorListItemViewModel> GetKeyResultIndicators()
        {
            return this.ResultIndicators.Where(r => r.IsKey).ToList();
        }

        public List<ResultIndicatorListItemViewModel> GetNonKeyResultIndicators()
        {
            return this.ResultIndicators.Where(r => !r.IsKey).ToList();
        }

        public List<PerformanceIndicatorListItemViewModel> GetKeyPerformanceIndicators()
        {
            return this.PerformanceIndicators.Where(p => p.IsKey).ToList();
        }

        public List<PerformanceIndicatorListItemViewModel> GetNonKeyPerformanceIndicators()
        {
            return this.PerformanceIndicators.Where(p => !p.IsKey).ToList();
        }

        public List<ResultIndicatorListItemViewModel> GetResultIndicatorsWithTarget()
        {
            return this.ResultIndicators.Where(r => r.TargetValue.HasValue).ToList();
        }

        public List<ResultIndicatorListItemViewModel> GetResultIndicatorsWithoutTarget()
        {
            return this.ResultIndicators.Where(r => !r.TargetValue.HasValue).ToList();
        }

        public List<PerformanceIndicatorListItemViewModel> GetPerformanceIndicatorsWithTarget()
        {
            return this.PerformanceIndicators.Where(p => p.TargetValue.HasValue).ToList();
        }

        public List<PerformanceIndicatorListItemViewModel> GetPerformanceIndicatorsWithoutTarget()
        {
            return this.PerformanceIndicators.Where(p => !p.TargetValue.HasValue).ToList();
        }

        // Summary properties
        public int TotalObjectives => this.Objectives.Count;
        public int TotalSuccessFactors => this.SuccessFactors.Count;
        public int TotalCriticalSuccessFactors => this.SuccessFactors.Count(sf => sf.IsCritical);
        public int TotalKeyResultIndicators => this.ResultIndicators.Count(ri => ri.IsKey);
        public int TotalKeyPerformanceIndicators => this.PerformanceIndicators.Count(pi => pi.IsKey);
        public int TotalResultIndicators => this.ResultIndicators.Count;
        public int TotalPerformanceIndicators => this.PerformanceIndicators.Count;
        public int TotalIndicators => this.TotalResultIndicators + this.TotalPerformanceIndicators;
    }
}