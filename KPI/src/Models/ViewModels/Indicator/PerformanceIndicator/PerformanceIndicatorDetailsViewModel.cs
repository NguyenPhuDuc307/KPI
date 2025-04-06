namespace KPISolution.Models.ViewModels.Indicator.PerformanceIndicator
{
    /// <summary>
    /// Detailed view model for displaying Performance Indicator details including all relationships and metrics
    /// </summary>
    public class PerformanceIndicatorDetailsViewModel
    {
        public Guid Id { get; init; }

        [Display(Name = "Name")]
        public string Name { get; init; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; init; }

        [Display(Name = "Code")]
        public string? Code { get; init; }

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

        [Display(Name = "Created By")]
        public string? CreatedBy { get; init; }

        [Display(Name = "Last Updated")]
        public DateTime? UpdatedAt { get; init; }

        [Display(Name = "Last Updated By")]
        public string? UpdatedBy { get; init; }

        // Relationships
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; init; }
        public string SuccessFactorName { get; init; } = string.Empty;
        public bool SuccessFactorIsCritical { get; init; }

        [Display(Name = "Responsible Team Member")]
        public Guid? ResponsibleTeamMemberId { get; init; }
        public string? ResponsibleTeamMemberName { get; init; }

        // Measurements
        public ICollection<MeasurementViewModel> RecentMeasurements { get; init; } = new List<MeasurementViewModel>();
        public int MeasurementCount { get; init; }
        public DateTime? LastMeasurementDate { get; init; }

        // Helper methods
        public string GetTypeDisplay() => this.IsKey ? "Key Performance Indicator (KPI)" : "Performance Indicator (PI)";

        public string GetTypeBadgeClass() => this.IsKey ? "bg-success" : "bg-primary";

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

        public bool IsInAlert()
        {
            if (!this.CurrentValue.HasValue || !this.AlertThreshold.HasValue)
                return false;

            // Assuming lower values are worse (common for performance indicators)
            return this.CurrentValue.Value <= this.AlertThreshold.Value;
        }

        public int GetProgressPercentage()
        {
            if (!this.CurrentValue.HasValue || !this.TargetValue.HasValue || this.TargetValue.Value == 0)
                return 0;

            var percentage = (int)(this.CurrentValue.Value / this.TargetValue.Value * 100);
            return Math.Min(percentage, 100); // Cap at 100%
        }

        public bool HasMeasurements => this.MeasurementCount > 0;

        public string GetAlertMessage()
        {
            if (!this.CurrentValue.HasValue || !this.TargetValue.HasValue)
                return "No measurements available yet. Add measurements to track progress.";

            if (this.IsInAlert())
                return "Warning: Current value has reached alert threshold level. Action required.";

            var percentage = this.CurrentValue.Value / this.TargetValue.Value * 100;

            return percentage switch
            {
                var p when p >= 100 => "Target achieved! Consider reviewing the target for the next period.",
                var p when p >= 75 => "Performance is on target.",
                var p when p >= 50 => "Performance is below target and requires attention.",
                _ => "Performance is significantly below target. Immediate action required."
            };
        }

        public string GetAlertClass()
        {
            if (!this.CurrentValue.HasValue || !this.TargetValue.HasValue)
                return "alert-info";

            if (this.IsInAlert())
                return "alert-danger";

            var percentage = this.CurrentValue.Value / this.TargetValue.Value * 100;

            return percentage switch
            {
                var p when p >= 100 => "alert-success",
                var p when p >= 75 => "alert-info",
                var p when p >= 50 => "alert-warning",
                _ => "alert-danger"
            };
        }
    }
}
