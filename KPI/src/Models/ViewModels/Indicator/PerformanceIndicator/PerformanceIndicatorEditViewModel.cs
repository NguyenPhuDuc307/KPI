using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Models.ViewModels.Indicator.PerformanceIndicator
{
    /// <summary>
    /// View model for creating and editing Performance Indicators (PI and KPI)
    /// </summary>
    public class PerformanceIndicatorEditViewModel
    {
        public Guid? Id { get; init; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; init; } = string.Empty;

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; init; }

        [Display(Name = "Code")]
        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        public string? Code { get; init; }

        [Display(Name = "Key Performance Indicator")]
        public bool IsKey { get; init; }

        [Display(Name = "Formula")]
        [StringLength(200, ErrorMessage = "Formula cannot exceed 200 characters")]
        public string? Formula { get; init; }

        [Display(Name = "Target Value")]
        [Range(0, double.MaxValue, ErrorMessage = "Target value must be greater than 0")]
        public decimal? TargetValue { get; init; }

        [Display(Name = "Alert Threshold")]
        [Range(0, double.MaxValue, ErrorMessage = "Alert threshold must be greater than 0")]
        public decimal? AlertThreshold { get; init; }

        [Required]
        [Display(Name = "Measurement Unit")]
        public MeasurementUnit Unit { get; init; }
        public IEnumerable<SelectListItem> UnitOptions { get; init; } = new List<SelectListItem>();

        [Required]
        [Display(Name = "Measurement Frequency")]
        public MeasurementFrequency Frequency { get; init; }
        public IEnumerable<SelectListItem> FrequencyOptions { get; init; } = new List<SelectListItem>();

        [Display(Name = "Review Frequency")]
        public ReviewFrequency? ReviewFrequency { get; init; }
        public IEnumerable<SelectListItem> ReviewFrequencyOptions { get; init; } = new List<SelectListItem>();

        [Display(Name = "Activity Type")]
        public ActivityType? ActivityType { get; init; }
        public IEnumerable<SelectListItem> ActivityTypeOptions { get; init; } = new List<SelectListItem>();

        [Display(Name = "Performance Level")]
        public PerformanceLevel? PerformanceLevel { get; init; }
        public IEnumerable<SelectListItem> PerformanceLevelOptions { get; init; } = new List<SelectListItem>();

        [Display(Name = "Control Level")]
        public ControlLevel? ControlLevel { get; init; }
        public IEnumerable<SelectListItem> ControlLevelOptions { get; init; } = new List<SelectListItem>();

        [Display(Name = "Action Plan")]
        [StringLength(500, ErrorMessage = "Action plan cannot exceed 500 characters")]
        public string? ActionPlan { get; init; }

        [Display(Name = "Data Collection Method")]
        public DataCollectionMethod? DataCollectionMethod { get; init; }
        public IEnumerable<SelectListItem> DataCollectionMethodOptions { get; init; } = new List<SelectListItem>();

        [Display(Name = "Contribution (%)")]
        [Range(0, 100, ErrorMessage = "Contribution percentage must be between 0 and 100")]
        public int? ContributionPercentage { get; init; }

        // Relationships
        [Required]
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; init; }
        public IEnumerable<SelectListItem> SuccessFactorOptions { get; init; } = new List<SelectListItem>();

        [Display(Name = "Responsible Team Member")]
        public Guid? ResponsibleTeamMemberId { get; init; }
        public IEnumerable<SelectListItem> ResponsibleTeamMemberOptions { get; init; } = new List<SelectListItem>();

        // UI helper properties
        public string PageTitle => this.Id.HasValue ? "Edit " + (this.IsKey ? "Key Performance Indicator" : "Performance Indicator") : "Create " + (this.IsKey ? "Key Performance Indicator" : "Performance Indicator");
        public string SubmitButtonText => this.Id.HasValue ? "Update" : "Create";
        public string CancelUrl => "/PerformanceIndicator";

        // Helper method to determine which fields should be shown based on type
        public bool ShowKpiSpecificFields => this.IsKey;
    }
}
