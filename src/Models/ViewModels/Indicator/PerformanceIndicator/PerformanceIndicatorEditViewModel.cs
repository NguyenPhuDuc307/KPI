using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Models.ViewModels.Indicator.PerformanceIndicator
{
    /// <summary>
    /// View model for creating and editing Performance Indicators (PI and KPI)
    /// </summary>
    public class PerformanceIndicatorEditViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Display(Name = "Code")]
        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        public string? Code { get; set; }

        [Display(Name = "Key Performance Indicator")]
        public bool IsKey { get; set; }

        [Display(Name = "Formula")]
        [StringLength(200, ErrorMessage = "Formula cannot exceed 200 characters")]
        public string? Formula { get; set; }

        [Display(Name = "Target Value")]
        [Range(0, double.MaxValue, ErrorMessage = "Target value must be greater than 0")]
        public decimal? TargetValue { get; set; }

        [Display(Name = "Ngưỡng cảnh báo thấp")]
        [Range(0, double.MaxValue, ErrorMessage = "Ngưỡng cảnh báo thấp phải lớn hơn 0")]
        public decimal? MinAlertThreshold { get; set; }

        [Display(Name = "Ngưỡng cảnh báo cao")]
        [Range(0, double.MaxValue, ErrorMessage = "Ngưỡng cảnh báo cao phải lớn hơn 0")]
        public decimal? MaxAlertThreshold { get; set; }

        [Required]
        [Display(Name = "Measurement Unit")]
        public MeasurementUnit Unit { get; set; }
        public IEnumerable<SelectListItem> UnitOptions { get; set; } = new List<SelectListItem>();

        [Required]
        [Display(Name = "Measurement Frequency")]
        public MeasurementFrequency Frequency { get; set; }
        public IEnumerable<SelectListItem> FrequencyOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Review Frequency")]
        public ReviewFrequency? ReviewFrequency { get; set; }
        public IEnumerable<SelectListItem> ReviewFrequencyOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Activity Type")]
        public ActivityType? ActivityType { get; set; }
        public IEnumerable<SelectListItem> ActivityTypeOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Control Level")]
        public ControlLevel? ControlLevel { get; set; }
        public IEnumerable<SelectListItem> ControlLevelOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Action Plan")]
        [StringLength(500, ErrorMessage = "Action plan cannot exceed 500 characters")]
        public string? ActionPlan { get; set; }

        [Display(Name = "Data Collection Method")]
        public DataCollectionMethod? DataCollectionMethod { get; set; }
        public IEnumerable<SelectListItem> DataCollectionMethodOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Contribution (%)")]
        [Range(0, 100, ErrorMessage = "Contribution percentage must be between 0 and 100")]
        public int? ContributionPercentage { get; set; }

        // Relationships
        [Required]
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; set; }
        public IEnumerable<SelectListItem> SuccessFactorOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Result Indicator")]
        public Guid? ResultIndicatorId { get; set; }
        public IEnumerable<SelectListItem> ResultIndicatorOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Responsible Team Member")]
        public Guid? ResponsibleTeamMemberId { get; set; }
        public IEnumerable<SelectListItem> ResponsibleTeamMemberOptions { get; set; } = new List<SelectListItem>();

        // UI helper properties
        public string PageTitle => this.Id.HasValue ? "Edit " + (this.IsKey ? "Key Performance Indicator" : "Performance Indicator") : "Create " + (this.IsKey ? "Key Performance Indicator" : "Performance Indicator");
        public string SubmitButtonText => this.Id.HasValue ? "Update" : "Create";
        public string CancelUrl => "/PerformanceIndicator";

        // Helper method to determine which fields should be shown based on type
        public bool ShowKpiSpecificFields => this.IsKey;
    }
}
