using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Indicator
{
    /// <summary>
    /// Represents a Result Indicator - measures what has been achieved in a specific area.
    /// Combined entity for both RI and KRI (Key Result Indicators) with IsKey flag.
    /// </summary>
    public class ResultIndicator : BaseEntity
    {
        /// <summary>
        /// Name of the Result Indicator
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Description of the Result Indicator
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string? Description { get; init; }

        /// <summary>
        /// Unique code for identifying this indicator
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Code")]
        public string Code { get; init; } = string.Empty;

        /// <summary>
        /// Indicates if this is a Key Result Indicator (true) or a regular Result Indicator (false)
        /// </summary>
        [Display(Name = "Is Key Result Indicator")]
        public bool IsKey { get; init; } = false;

        /// <summary>
        /// Formula used to calculate the indicator value
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Formula")]
        public string? Formula { get; init; }

        /// <summary>
        /// Target or goal value for the indicator
        /// </summary>
        [Display(Name = "Target Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? TargetValue { get; init; }

        /// <summary>
        /// Current value of the indicator
        /// </summary>
        [Display(Name = "Current Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? CurrentValue { get; set; }

        /// <summary>
        /// Unit of measurement (e.g., percentage, days, count)
        /// </summary>
        [Required]
        [StringLength(30)]
        [Display(Name = "Unit")]
        public string Unit { get; init; } = "%";

        /// <summary>
        /// Minimum acceptable value for the indicator
        /// </summary>
        [Display(Name = "Minimum Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? MinimumValue { get; init; }

        /// <summary>
        /// Maximum possible value for the indicator
        /// </summary>
        [Display(Name = "Maximum Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? MaximumValue { get; init; }

        /// <summary>
        /// Weight or importance of this indicator (often used for calculations)
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Weight (%)")]
        public int? Weight { get; init; }

        /// <summary>
        /// Frequency of measurement (daily, weekly, monthly, quarterly, annually)
        /// </summary>
        [Required]
        [Display(Name = "Frequency")]
        public MeasurementFrequency Frequency { get; init; } = MeasurementFrequency.Monthly;

        /// <summary>
        /// Measurement direction indicating whether higher or lower values are better
        /// </summary>
        [Required]
        [Display(Name = "Measurement Direction")]
        public MeasurementDirection MeasurementDirection { get; init; } = MeasurementDirection.HigherIsBetter;

        /// <summary>
        /// Current performance trend
        /// </summary>
        [Display(Name = "Performance Trend")]
        public PerformanceTrend PerformanceTrend { get; init; } = PerformanceTrend.NotSet;

        /// <summary>
        /// Process area this Result Indicator belongs to
        /// </summary>
        [Display(Name = "Process Area")]
        public ProcessArea? ProcessArea { get; init; }

        /// <summary>
        /// Link to the Success Factor (SF) or Critical Success Factor (CSF)
        /// </summary>
        [Required]
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; init; }

        /// <summary>
        /// Navigation property to the Success Factor
        /// </summary>
        [ForeignKey("SuccessFactorId")]
        public virtual SuccessFactor SuccessFactor { get; init; } = null!;

        /// <summary>
        /// Person responsible for this indicator
        /// </summary>
        [Display(Name = "Responsible Person")]
        public string? ResponsiblePersonId { get; init; }

        /// <summary>
        /// Navigation property to the responsible person
        /// </summary>
        [ForeignKey("ResponsiblePersonId")]
        public virtual ApplicationUser? ResponsiblePerson { get; init; }

        /// <summary>
        /// Scope or boundary of what this indicator is measuring
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Measurement Scope")]
        public string? MeasurementScope { get; init; }

        /// <summary>
        /// The timeframe for which this result is relevant
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Time Frame")]
        public string? TimeFrame { get; init; }

        /// <summary>
        /// Primary source of data for this indicator
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Data Source")]
        public string? DataSource { get; init; }

        /// <summary>
        /// Type of result (e.g., Financial, Customer Satisfaction)
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Result Type")]
        public string? ResultType { get; init; }

        /// <summary>
        /// Current status of this indicator
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public IndicatorStatus Status { get; init; } = IndicatorStatus.Draft;

        /// <summary>
        /// Collection of related Performance Indicators that contribute to this Result Indicator
        /// </summary>
        public virtual ICollection<PerformanceIndicator>? PerformanceIndicators { get; init; }

        /// <summary>
        /// Collection of measurement values
        /// </summary>
        public virtual ICollection<Measurement>? Measurements { get; init; }

        /// <summary>
        /// Tần suất đo lường
        /// </summary>
        [Display(Name = "Tần suất đo lường")]
        public MeasurementFrequency MeasurementFrequency { get; init; } = MeasurementFrequency.Monthly;

        /// <summary>
        /// Thời gian cập nhật lần cuối
        /// </summary>
        [Display(Name = "Cập nhật lần cuối")]
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// ID phòng ban
        /// </summary>
        public Guid? DepartmentId { get; init; }

        /// <summary>
        /// Tham chiếu đến phòng ban
        /// </summary>
        public virtual Department? Department { get; init; }
    }
}
