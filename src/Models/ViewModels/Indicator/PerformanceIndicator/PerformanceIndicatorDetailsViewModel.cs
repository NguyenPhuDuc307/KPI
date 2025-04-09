namespace KPISolution.Models.ViewModels.Indicator.PerformanceIndicator
{
    /// <summary>
    /// Detailed view model for displaying Performance Indicator details including all relationships and metrics
    /// </summary>
    public class PerformanceIndicatorDetailsViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Code")]
        public string? Code { get; set; }

        [Display(Name = "Key Performance Indicator")]
        public bool IsKey { get; set; }

        [Display(Name = "Formula")]
        public string? Formula { get; set; }

        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }

        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }

        [Display(Name = "Ngưỡng cảnh báo tối thiểu")]
        public decimal? MinAlertThreshold { get; set; }

        [Display(Name = "Ngưỡng cảnh báo tối đa")]
        public decimal? MaxAlertThreshold { get; set; }

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

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Last Updated By")]
        public string? UpdatedBy { get; set; }

        // Relationships
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; set; }
        public string SuccessFactorName { get; set; } = string.Empty;
        public bool SuccessFactorIsCritical { get; set; }

        [Display(Name = "Result Indicator")]
        public Guid? ResultIndicatorId { get; set; }
        public string? ResultIndicatorName { get; set; }

        [Display(Name = "Responsible Team Member")]
        public Guid? ResponsibleTeamMemberId { get; set; }
        public string? ResponsibleTeamMemberName { get; set; }

        // Measurements
        public ICollection<MeasurementViewModel> RecentMeasurements { get; set; } = new List<MeasurementViewModel>();
        public int MeasurementCount { get; set; }
        public DateTime? LastMeasurementDate { get; set; }

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
            if (!this.CurrentValue.HasValue)
                return false;

            if (this.MinAlertThreshold.HasValue && this.CurrentValue.Value <= this.MinAlertThreshold.Value)
                return true;

            if (this.MaxAlertThreshold.HasValue && this.CurrentValue.Value >= this.MaxAlertThreshold.Value)
                return true;

            return false;
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

            if (this.MinAlertThreshold.HasValue && this.CurrentValue.Value <= this.MinAlertThreshold.Value)
                return "Warning: Current value has fallen below minimum threshold. Action required.";

            if (this.MaxAlertThreshold.HasValue && this.CurrentValue.Value >= this.MaxAlertThreshold.Value)
                return "Warning: Current value has exceeded maximum threshold. Action required.";

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
