namespace KPISolution.Models.ViewModels.Indicator.PerformanceIndicator
{
    /// <summary>
    /// Simplified view model for displaying Performance Indicators in lists
    /// </summary>
    public class PerformanceIndicatorListItemViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Key Performance Indicator")]
        public bool IsKey { get; set; }

        [Display(Name = "Target")]
        public decimal? TargetValue { get; set; }

        [Display(Name = "Ngưỡng cảnh báo tối thiểu")]
        public decimal? MinAlertThreshold { get; set; }

        [Display(Name = "Ngưỡng cảnh báo tối đa")]
        public decimal? MaxAlertThreshold { get; set; }

        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }

        [Display(Name = "Unit")]
        public MeasurementUnit Unit { get; set; }
        public string UnitDisplay => this.Unit.ToString();

        [Display(Name = "Frequency")]
        public MeasurementFrequency Frequency { get; set; }
        public string FrequencyDisplay => this.Frequency.ToString();

        [Display(Name = "Activity Type")]
        public ActivityType? ActivityType { get; set; }
        public string ActivityTypeDisplay => this.ActivityType?.ToString() ?? string.Empty;

        [Display(Name = "Formula")]
        public string Formula { get; set; } = string.Empty;

        // Relationships
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; set; }
        public string SuccessFactorName { get; set; } = string.Empty;
        public bool SuccessFactorIsCritical { get; set; }

        [Display(Name = "Result Indicator")]
        public Guid? ResultIndicatorId { get; set; }
        public string ResultIndicatorName { get; set; } = string.Empty;
        public bool ResultIndicatorIsKey { get; set; }

        // Measurement info
        public int MeasurementCount { get; set; }
        public DateTime? LastMeasurementDate { get; set; }

        // Helper methods
        public string GetTypeDisplay() => this.IsKey ? "KPI" : "PI";
        public string GetTypeBadgeClass() => this.IsKey ? "bg-success" : "bg-primary";

        public string GetValueStatus()
        {
            if (!this.CurrentValue.HasValue || !this.TargetValue.HasValue)
                return "Not Available";

            var percentage = (this.CurrentValue.Value / this.TargetValue.Value) * 100;

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

            var percentage = (this.CurrentValue.Value / this.TargetValue.Value) * 100;

            return percentage switch
            {
                var p when p >= 100 => "bg-success",
                var p when p >= 75 => "bg-info",
                var p when p >= 50 => "bg-warning",
                _ => "bg-danger"
            };
        }

        public string GetStatusBadgeClass()
        {
            var status = GetValueStatus();
            return status switch
            {
                "Achieved" => "bg-success",
                "On Target" => "bg-info",
                "Below Target" => "bg-warning",
                "At Risk" => "bg-danger",
                _ => "bg-secondary"
            };
        }

        public string GetStatusDisplay()
        {
            var status = GetValueStatus();
            return status switch
            {
                "Achieved" => "Đạt",
                "On Target" => "Đúng mục tiêu",
                "Below Target" => "Dưới mục tiêu",
                "At Risk" => "Rủi ro",
                _ => "Chưa có dữ liệu"
            };
        }
    }
}
