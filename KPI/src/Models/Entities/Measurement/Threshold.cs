using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Measurement
{
    /// <summary>
    /// Represents threshold values for an indicator that determine its status
    /// </summary>
    public class Threshold : BaseEntity
    {
        /// <summary>
        /// The type of indicator this threshold is for
        /// </summary>
        [Required]
        [Display(Name = "Indicator Type")]
        public IndicatorMeasurementType IndicatorType { get; init; }

        /// <summary>
        /// Minimum value for "Red" status (critical/poor performance)
        /// </summary>
        [Display(Name = "Red Threshold")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal RedThreshold { get; init; }

        /// <summary>
        /// Minimum value for "Yellow" status (warning/mediocre performance)
        /// </summary>
        [Display(Name = "Yellow Threshold")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal YellowThreshold { get; init; }

        /// <summary>
        /// Minimum value for "Green" status (good/acceptable performance)
        /// </summary>
        [Display(Name = "Green Threshold")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal GreenThreshold { get; init; }

        /// <summary>
        /// Indicates if higher values are better for this indicator
        /// </summary>
        [Display(Name = "Higher is Better")]
        public bool HigherIsBetter { get; init; } = true;

        /// <summary>
        /// Description of what red status means for this indicator
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Red Description")]
        public string? RedDescription { get; init; }

        /// <summary>
        /// Description of what yellow status means for this indicator
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Yellow Description")]
        public string? YellowDescription { get; init; }

        /// <summary>
        /// Description of what green status means for this indicator
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Green Description")]
        public string? GreenDescription { get; init; }

        /// <summary>
        /// Effective date for these thresholds
        /// </summary>
        [Required]
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Expiration date for these thresholds
        /// </summary>
        [Display(Name = "Expiration Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; init; }

        /// <summary>
        /// Person who created these thresholds
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Created By")]
        public string? CreatedById { get; init; }

        /// <summary>
        /// Navigation property to the user who created these thresholds
        /// </summary>
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser? CreatedByUser { get; init; }

        #region IndicatorRelationships

        /// <summary>
        /// ID of the Success Factor this threshold belongs to (optional)
        /// </summary>
        [Display(Name = "Success Factor")]
        public Guid? SuccessFactorId { get; init; }

        /// <summary>
        /// Navigation property to the Success Factor
        /// </summary>
        [ForeignKey("SuccessFactorId")]
        public virtual SuccessFactor? SuccessFactor { get; init; }

        /// <summary>
        /// ID of the Result Indicator this threshold belongs to (optional)
        /// </summary>
        [Display(Name = "Result Indicator")]
        public Guid? ResultIndicatorId { get; init; }

        /// <summary>
        /// Navigation property to the Result Indicator
        /// </summary>
        [ForeignKey("ResultIndicatorId")]
        public virtual ResultIndicator? ResultIndicator { get; init; }

        /// <summary>
        /// ID of the Performance Indicator this threshold belongs to (optional)
        /// </summary>
        [Display(Name = "Performance Indicator")]
        public Guid? PerformanceIndicatorId { get; init; }

        /// <summary>
        /// Navigation property to the Performance Indicator
        /// </summary>
        [ForeignKey("PerformanceIndicatorId")]
        public virtual PerformanceIndicator? PerformanceIndicator { get; init; }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the ID of the associated indicator
        /// </summary>
        /// <returns>The ID of the indicator</returns>
        public Guid GetIndicatorId()
        {
            if (this.SuccessFactorId.HasValue)
                return this.SuccessFactorId.Value;

            if (this.ResultIndicatorId.HasValue)
                return this.ResultIndicatorId.Value;

            if (this.PerformanceIndicatorId.HasValue)
                return this.PerformanceIndicatorId.Value;

            throw new InvalidOperationException("Threshold must be associated with an indicator");
        }

        /// <summary>
        /// Gets the name of the associated indicator (requires navigation properties to be loaded)
        /// </summary>
        /// <returns>The name of the indicator or null if navigation property not loaded</returns>
        public string? GetIndicatorName()
        {
            if (this.SuccessFactor != null)
                return this.SuccessFactor.Name;

            if (this.ResultIndicator != null)
                return this.ResultIndicator.Name;

            if (this.PerformanceIndicator != null)
                return this.PerformanceIndicator.Name;

            return null;
        }

        /// <summary>
        /// Determines the status for a given value based on the thresholds
        /// </summary>
        /// <param name="value">The value to evaluate</param>
        /// <returns>The status based on the thresholds</returns>
        public string DetermineStatus(decimal value)
        {
            if (this.HigherIsBetter)
            {
                if (value >= this.GreenThreshold)
                    return "Green";
                if (value >= this.YellowThreshold)
                    return "Yellow";
                return "Red";
            }

            if (value <= this.GreenThreshold)
                return "Green";
            if (value <= this.YellowThreshold)
                return "Yellow";
            return "Red";
        }

        #endregion
    }
}
