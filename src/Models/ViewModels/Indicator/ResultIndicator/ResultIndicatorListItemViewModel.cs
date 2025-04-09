namespace KPISolution.Models.ViewModels.Indicator.ResultIndicator
{
    /// <summary>
    /// Simplified view model for displaying Result Indicators in lists
    /// </summary>
    public class ResultIndicatorListItemViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Key Result Indicator")]
        public bool IsKey { get; set; }

        [Display(Name = "Target")]
        public decimal? TargetValue { get; set; }

        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }

        [Display(Name = "Unit")]
        public MeasurementUnit Unit { get; set; }
        public string UnitDisplay => this.Unit.ToString();

        [Display(Name = "Frequency")]
        public MeasurementFrequency Frequency { get; set; }
        public string FrequencyDisplay => this.Frequency.ToString();

        // Relationships
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; set; }
        public string SuccessFactorName { get; set; } = string.Empty;
        public bool SuccessFactorIsCritical { get; set; }

        // Measurement info
        public int MeasurementCount { get; set; }
        public DateTime? LastMeasurementDate { get; set; }

        // Helper methods
        public string GetTypeDisplay() => this.IsKey ? "KRI" : "RI";
        public string GetTypeBadgeClass() => this.IsKey ? "bg-danger" : "bg-info";

        public string GetValueStatus()
        {
            if (!this.CurrentValue.HasValue || !this.TargetValue.HasValue)
                return "Not Available";

            var percentage = (this.CurrentValue.Value / this.TargetValue.Value) * 100;

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

            var percentage = (this.CurrentValue.Value / this.TargetValue.Value) * 100;

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
