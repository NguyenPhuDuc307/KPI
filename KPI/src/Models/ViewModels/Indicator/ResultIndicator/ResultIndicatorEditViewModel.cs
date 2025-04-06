using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Models.ViewModels.Indicator.ResultIndicator
{
    /// <summary>
    /// View model for creating and editing Result Indicators (RI and KRI)
    /// </summary>
    public class ResultIndicatorEditViewModel
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

        [Display(Name = "Key Result Indicator")]
        public bool IsKey { get; set; }

        [Display(Name = "Formula")]
        [StringLength(200, ErrorMessage = "Formula cannot exceed 200 characters")]
        public string? Formula { get; set; }

        [Display(Name = "Target Value")]
        [Range(0, double.MaxValue, ErrorMessage = "Target value must be greater than 0")]
        public decimal? TargetValue { get; set; }

        [Required]
        [Display(Name = "Measurement Unit")]
        public MeasurementUnit Unit { get; set; }
        public IEnumerable<SelectListItem> UnitOptions { get; set; } = new List<SelectListItem>();

        [Required]
        [Display(Name = "Measurement Frequency")]
        public MeasurementFrequency Frequency { get; set; }
        public IEnumerable<SelectListItem> FrequencyOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Measurement Scope")]
        public MeasurementScope? MeasurementScope { get; set; }
        public IEnumerable<SelectListItem> MeasurementScopeOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Process Area")]
        public ProcessArea? ProcessArea { get; set; }
        public IEnumerable<SelectListItem> ProcessAreaOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Time Frame")]
        public TimeFrame? TimeFrame { get; set; }
        public IEnumerable<SelectListItem> TimeFrameOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Data Source")]
        public DataSource? DataSource { get; set; }
        public IEnumerable<SelectListItem> DataSourceOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Result Type")]
        public ResultType? ResultType { get; set; }
        public IEnumerable<SelectListItem> ResultTypeOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Contribution (%)")]
        [Range(0, 100, ErrorMessage = "Contribution percentage must be between 0 and 100")]
        public int? ContributionPercentage { get; set; }

        // Relationships
        [Required]
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; set; }
        public IEnumerable<SelectListItem> SuccessFactorOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Responsible Manager")]
        public Guid? ResponsibleManagerId { get; set; }
        public IEnumerable<SelectListItem> ResponsibleManagerOptions { get; set; } = new List<SelectListItem>();

        // UI helper properties
        public string PageTitle => this.Id.HasValue ? "Edit " + (this.IsKey ? "Key Result Indicator" : "Result Indicator") : "Create " + (this.IsKey ? "Key Result Indicator" : "Result Indicator");
        public string SubmitButtonText => this.Id.HasValue ? "Update" : "Create";
        public string CancelUrl => "/ResultIndicator";

        // Helper method to determine which fields should be shown based on type
        public bool ShowKriSpecificFields => this.IsKey;
    }
}
