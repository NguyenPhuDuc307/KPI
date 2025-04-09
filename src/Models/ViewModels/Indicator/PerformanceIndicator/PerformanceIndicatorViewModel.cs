namespace KPISolution.Models.ViewModels.Indicator.PerformanceIndicator
{
    /// <summary>
    /// View model for displaying Performance Indicators, includes both PI and KPI distinguished by IsKey flag
    /// </summary>
    public class PerformanceIndicatorViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Key Performance Indicator")]
        public bool IsKey { get; set; }

        [Display(Name = "Formula")]
        public string? Formula { get; set; }

        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }

        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }

        [Display(Name = "Alert Threshold")]
        public decimal? AlertThreshold { get; set; }

        [Display(Name = "Measurement Unit")]
        public MeasurementUnit Unit { get; set; }
        public string UnitDisplay => this.Unit.ToString();

        [Display(Name = "Measurement Frequency")]
        public MeasurementFrequency Frequency { get; set; }
        public string FrequencyDisplay => this.Frequency.ToString();

        [Display(Name = "Review Frequency")]
        public ReviewFrequency? ReviewFrequency { get; set; }
        public string ReviewFrequencyDisplay => this.ReviewFrequency?.ToString() ?? string.Empty;

        [Display(Name = "Activity Type")]
        public ActivityType? ActivityType { get; set; }
        public string ActivityTypeDisplay => this.ActivityType?.ToString() ?? string.Empty;

        [Display(Name = "Performance Level")]
        public PerformanceLevel? PerformanceLevel { get; set; }
        public string PerformanceLevelDisplay => this.PerformanceLevel?.ToString() ?? string.Empty;

        [Display(Name = "Control Level")]
        public ControlLevel? ControlLevel { get; set; }
        public string ControlLevelDisplay => this.ControlLevel?.ToString() ?? string.Empty;

        [Display(Name = "Action Plan")]
        public string? ActionPlan { get; set; }

        [Display(Name = "Data Collection Method")]
        public DataCollectionMethod? DataCollectionMethod { get; set; }
        public string DataCollectionMethodDisplay => this.DataCollectionMethod?.ToString() ?? string.Empty;

        [Display(Name = "Contribution (%)")]
        public int? ContributionPercentage { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime? UpdatedAt { get; set; }

        // Relationships
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; set; }
        public string SuccessFactorName { get; set; } = string.Empty;
        public bool SuccessFactorIsCritical { get; set; }

        [Display(Name = "Responsible Team Member")]
        public Guid? ResponsibleTeamMemberId { get; set; }
        public string? ResponsibleTeamMemberName { get; set; }

        // Navigation collections
        public int MeasurementCount { get; set; }

        // Helper methods
        public string GetTypeDisplay() => this.IsKey ? "Key Performance Indicator (KPI)" : "Performance Indicator (PI)";

        public string GetValueStatusDisplay()
        {
            if (!this.CurrentValue.HasValue || !this.TargetValue.HasValue)
                return "Not Available";

            var percentage = this.CurrentValue.Value / this.TargetValue.Value * 100;

            return percentage switch
            {
                var p when p >= 100 => "Achieved",
                var p when p >= 75 => "On Target",
                var p when p >= 50 => "Below Target",
                _ => "At Risk"
            };
        }

        public string GetValueStatusClass()
        {
            if (!this.CurrentValue.HasValue || !this.TargetValue.HasValue)
                return "bg-secondary";

            var percentage = this.CurrentValue.Value / this.TargetValue.Value * 100;

            return percentage switch
            {
                var p when p >= 100 => "bg-success",
                var p when p >= 75 => "bg-info",
                var p when p >= 50 => "bg-warning",
                _ => "bg-danger"
            };
        }
    }
}
