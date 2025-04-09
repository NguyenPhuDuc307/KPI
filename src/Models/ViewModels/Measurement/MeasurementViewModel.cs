namespace KPISolution.Models.ViewModels.Measurement
{
    /// <summary>
    /// View model for displaying a measurement
    /// </summary>
    public class MeasurementViewModel : AbstractMeasurementViewModel
    {
        /// <summary>
        /// Indicator ID
        /// </summary>
        public Guid IndicatorId { get; set; }

        /// <summary>
        /// Name of the indicator
        /// </summary>
        public string IndicatorName { get; set; } = string.Empty;

        /// <summary>
        /// Type of indicator (KPI, PI, KRI, RI)
        /// </summary>
        public string IndicatorType { get; set; } = string.Empty;

        /// <summary>
        /// Department name
        /// </summary>
        public string? DepartmentName { get; set; }

        /// <summary>
        /// Measurement unit
        /// </summary>
        public string IndicatorUnit { get; set; } = string.Empty;
        
        /// <summary>
        /// Date and time the measurement was created
        /// </summary>
        public DateTime? CreatedAt { get; set; }
        
        /// <summary>
        /// User who created the measurement
        /// </summary>
        public string? CreatedBy { get; set; }
        
        /// <summary>
        /// Date and time the measurement was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// User who last updated the measurement
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Calculate achievement percentage
        /// </summary>
        public decimal AchievementPercentage => this.CalculateAchievementPercentage();

        /// <summary>
        /// Variance from target
        /// </summary>
        public decimal Variance => this.CalculateVariance();

        /// <summary>
        /// Status text based on achievement
        /// </summary>
        public string StatusText
        {
            get
            {
                if (!this.TargetValue.HasValue) return "Not Set";

                if (this.Status != MeasurementStatus.NotSet)
                {
                    return this.Status.ToString();
                }

                var percentage = this.AchievementPercentage;
                if (percentage >= 100) return "On Target";
                if (percentage >= 80) return "At Risk";
                return "Below Target";
            }
        }
    }
}
