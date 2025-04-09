namespace KPISolution.Models.ViewModels.Indicator.ResultIndicator
{
    /// <summary>
    /// Detailed view model for displaying Result Indicator details including all relationships and metrics
    /// </summary>
    public class ResultIndicatorDetailsViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Code")]
        public string? Code { get; set; }

        [Display(Name = "Key Result Indicator")]
        public bool IsKey { get; set; }

        [Display(Name = "Formula")]
        public string? Formula { get; set; }

        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }

        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }

        [Display(Name = "Measurement Unit")]
        public MeasurementUnit Unit { get; set; }
        public string UnitDisplay => this.Unit.ToString();

        [Display(Name = "Measurement Frequency")]
        public MeasurementFrequency Frequency { get; set; }
        public string FrequencyDisplay => this.Frequency.ToString();

        [Display(Name = "Measurement Scope")]
        public MeasurementScope? MeasurementScope { get; set; }
        public string MeasurementScopeDisplay => this.MeasurementScope?.ToString() ?? string.Empty;

        [Display(Name = "Process Area")]
        public ProcessArea? ProcessArea { get; set; }
        public string ProcessAreaDisplay => this.ProcessArea?.ToString() ?? string.Empty;

        [Display(Name = "Time Frame")]
        public TimeFrame? TimeFrame { get; set; }
        public string TimeFrameDisplay => this.TimeFrame?.ToString() ?? string.Empty;

        [Display(Name = "Data Source")]
        public DataSource? DataSource { get; set; }
        public string DataSourceDisplay => this.DataSource?.ToString() ?? string.Empty;

        [Display(Name = "Result Type")]
        public ResultType? ResultType { get; set; }
        public string ResultTypeDisplay => this.ResultType?.ToString() ?? string.Empty;

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

        [Display(Name = "Responsible Manager")]
        public Guid? ResponsibleManagerId { get; set; }
        public string? ResponsibleManagerName { get; set; }

        [Display(Name = "Người phụ trách")]
        public string? ResponsiblePersonId { get; set; }
        public string? ResponsiblePersonName { get; set; }

        // Measurements
        public ICollection<MeasurementViewModel> RecentMeasurements { get; set; } = new List<MeasurementViewModel>();
        public int MeasurementCount { get; set; }
        public DateTime? LastMeasurementDate { get; set; }

        // Associated Performance Indicators
        public List<PerformanceIndicatorListItemViewModel> PerformanceIndicators { get; set; } = new List<PerformanceIndicatorListItemViewModel>();

        // Helper methods
        public string GetTypeDisplay() => this.IsKey ? "Key Result Indicator (KRI)" : "Result Indicator (RI)";

        public string GetTypeBadgeClass() => this.IsKey ? "bg-danger" : "bg-info";

        public string GetValueStatusDisplay()
        {
            if (!this.CurrentValue.HasValue || !this.TargetValue.HasValue)
                return "Not Available";

            var percentage = this.CurrentValue.Value / this.TargetValue.Value * 100;

            return percentage switch
            {
                var p when p >= 100 => "Achieved",
                var p when p >= 75 => "On Track",
                var p when p >= 50 => "Needs Attention",
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

            var percentage = this.CurrentValue.Value / this.TargetValue.Value * 100;

            return percentage switch
            {
                var p when p >= 100 => "Target achieved! Consider reviewing the target for the next period.",
                var p when p >= 75 => "Good progress toward target.",
                var p when p >= 50 => "Making progress, but needs attention to reach target.",
                _ => "Significant attention required - far below target."
            };
        }

        public string GetAlertClass()
        {
            if (!this.CurrentValue.HasValue || !this.TargetValue.HasValue)
                return "alert-info";

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
