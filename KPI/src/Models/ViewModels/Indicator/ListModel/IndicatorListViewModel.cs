using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Models.ViewModels.Indicator.ListModel
{
    /// <summary>
    /// View model for displaying and filtering lists of indicators
    /// </summary>
    public class IndicatorListViewModel
    {
        public enum IndicatorListType
        {
            SuccessFactor,
            ResultIndicator,
            PerformanceIndicator,
            Mixed
        }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IndicatorListType ListType { get; set; }

        // Success Factor collections
        public ICollection<SuccessFactorListItemViewModel> SuccessFactors { get; set; } = new List<SuccessFactorListItemViewModel>();
        public ICollection<SuccessFactorListItemViewModel> CriticalSuccessFactors { get; set; } = new List<SuccessFactorListItemViewModel>();

        // Result Indicator collections
        public ICollection<ResultIndicatorListItemViewModel> ResultIndicators { get; set; } = new List<ResultIndicatorListItemViewModel>();
        public ICollection<ResultIndicatorListItemViewModel> KeyResultIndicators { get; set; } = new List<ResultIndicatorListItemViewModel>();

        // Performance Indicator collections
        public ICollection<PerformanceIndicatorListItemViewModel> PerformanceIndicators { get; set; } = new List<PerformanceIndicatorListItemViewModel>();
        public ICollection<PerformanceIndicatorListItemViewModel> KeyPerformanceIndicators { get; set; } = new List<PerformanceIndicatorListItemViewModel>();

        // Filter properties
        [Display(Name = "Filter Type")]
        public string? FilterType { get; set; }
        public IEnumerable<SelectListItem> FilterTypeOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }
        public IEnumerable<SelectListItem> DepartmentOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Objective")]
        public Guid? ObjectiveId { get; set; }
        public IEnumerable<SelectListItem> ObjectiveOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Success Factor")]
        public Guid? SuccessFactorId { get; set; }
        public IEnumerable<SelectListItem> SuccessFactorOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Status")]
        public string? StatusFilter { get; set; }
        public IEnumerable<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Performance Level")]
        public string? PerformanceFilter { get; set; }
        public IEnumerable<SelectListItem> PerformanceOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }

        // Helper properties
        public int TotalCount => this.GetTotalCount();

        public bool HasSuccessFactors => this.SuccessFactors?.Count > 0 || this.CriticalSuccessFactors?.Count > 0;
        public bool HasResultIndicators => this.ResultIndicators?.Count > 0 || this.KeyResultIndicators?.Count > 0;
        public bool HasPerformanceIndicators => this.PerformanceIndicators?.Count > 0 || this.KeyPerformanceIndicators?.Count > 0;

        public int SuccessFactorCount => this.SuccessFactors?.Count ?? 0;
        public int CriticalSuccessFactorCount => this.CriticalSuccessFactors?.Count ?? 0;
        public int ResultIndicatorCount => this.ResultIndicators?.Count ?? 0;
        public int KeyResultIndicatorCount => this.KeyResultIndicators?.Count ?? 0;
        public int PerformanceIndicatorCount => this.PerformanceIndicators?.Count ?? 0;
        public int KeyPerformanceIndicatorCount => this.KeyPerformanceIndicators?.Count ?? 0;

        private int GetTotalCount()
        {
            return this.SuccessFactorCount + this.CriticalSuccessFactorCount + this.ResultIndicatorCount + this.KeyResultIndicatorCount + this.PerformanceIndicatorCount + this.KeyPerformanceIndicatorCount;
        }

        // UI helper properties
        public bool ShowFilters { get; set; } = true;
        public bool ShowCreateButtons { get; set; } = true;
        public string EmptyMessage { get; set; } = "No indicators found matching the current filters.";
    }
}
