namespace KPISolution.Models.ViewModels.Indicator.PerformanceIndicator
{
    /// <summary>
    /// View model for displaying Performance Indicators, includes both PI and KPI distinguished by IsKey flag
    /// </summary>
    public class PerformanceIndicatorViewModel
    {
        public Guid Id { get; init; }

        [Display(Name = "Name")]
        public string Name { get; init; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; init; }

        [Display(Name = "Code")]
        public string Code { get; init; } = string.Empty;

        [Display(Name = "Key Performance Indicator")]
        public bool IsKey { get; init; }

        [Display(Name = "Formula")]
        public string? Formula { get; init; }

        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; init; }

        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; init; }

        [Display(Name = "Alert Threshold")]
        public decimal? AlertThreshold { get; init; }

        [Display(Name = "Measurement Unit")]
        public MeasurementUnit Unit { get; init; }
        public string UnitDisplay => this.Unit.ToString();

        [Display(Name = "Measurement Frequency")]
        public MeasurementFrequency Frequency { get; init; }
        public string FrequencyDisplay => this.Frequency.ToString();

        [Display(Name = "Review Frequency")]
        public ReviewFrequency? ReviewFrequency { get; init; }
        public string ReviewFrequencyDisplay => this.ReviewFrequency?.ToString() ?? string.Empty;

        [Display(Name = "Activity Type")]
        public ActivityType? ActivityType { get; init; }
        public string ActivityTypeDisplay => this.ActivityType?.ToString() ?? string.Empty;

        [Display(Name = "Performance Level")]
        public PerformanceLevel? PerformanceLevel { get; init; }
        public string PerformanceLevelDisplay => this.PerformanceLevel?.ToString() ?? string.Empty;

        [Display(Name = "Control Level")]
        public ControlLevel? ControlLevel { get; init; }
        public string ControlLevelDisplay => this.ControlLevel?.ToString() ?? string.Empty;

        [Display(Name = "Action Plan")]
        public string? ActionPlan { get; init; }

        [Display(Name = "Data Collection Method")]
        public DataCollectionMethod? DataCollectionMethod { get; init; }
        public string DataCollectionMethodDisplay => this.DataCollectionMethod?.ToString() ?? string.Empty;

        [Display(Name = "Contribution (%)")]
        public int? ContributionPercentage { get; init; }

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; init; }

        [Display(Name = "Last Updated")]
        public DateTime? UpdatedAt { get; init; }

        // Relationships
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; init; }
        public string SuccessFactorName { get; init; } = string.Empty;
        public bool SuccessFactorIsCritical { get; init; }

        [Display(Name = "Responsible Team Member")]
        public Guid? ResponsibleTeamMemberId { get; init; }
        public string? ResponsibleTeamMemberName { get; init; }

        // Navigation collections
        public int MeasurementCount { get; init; }

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
