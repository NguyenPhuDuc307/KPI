namespace KPISolution.Models.ViewModels.Indicator.ResultIndicator
{
    /// <summary>
    /// View model for displaying Result Indicators, includes both RI and KRI distinguished by IsKey flag
    /// </summary>
    public class ResultIndicatorViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

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

        [Display(Name = "Last Updated")]
        public DateTime? UpdatedAt { get; set; }

        // Relationships
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; set; }
        public string SuccessFactorName { get; set; } = string.Empty;
        public bool SuccessFactorIsCritical { get; set; }

        [Display(Name = "Responsible Manager")]
        public Guid? ResponsibleManagerId { get; set; }
        public string? ResponsibleManagerName { get; set; }

        // Navigation collections
        public int MeasurementCount { get; set; }

        // Helper methods
        public string GetTypeDisplay() => this.IsKey ? "Key Result Indicator (KRI)" : "Result Indicator (RI)";

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
    }
}
