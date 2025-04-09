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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Result Indicator
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Unique code for identifying this indicator
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if this is a Key Result Indicator (true) or a regular Result Indicator (false)
        /// </summary>
        [Display(Name = "Is Key Result Indicator")]
        public bool IsKey { get; set; } = false;

        /// <summary>
        /// Formula used to calculate the indicator value
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Formula")]
        public string? Formula { get; set; }

        /// <summary>
        /// Target or goal value for the indicator
        /// </summary>
        [Display(Name = "Target Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? TargetValue { get; set; }

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
        public string Unit { get; set; } = "%";

        /// <summary>
        /// Minimum acceptable value for the indicator
        /// </summary>
        [Display(Name = "Minimum Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? MinimumValue { get; set; }

        /// <summary>
        /// Maximum possible value for the indicator
        /// </summary>
        [Display(Name = "Maximum Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? MaximumValue { get; set; }

        /// <summary>
        /// Weight or importance of this indicator (often used for calculations)
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Weight (%)")]
        public int? Weight { get; set; }

        /// <summary>
        /// Frequency of measurement (daily, weekly, monthly, quarterly, annually)
        /// </summary>
        [Required]
        [Display(Name = "Frequency")]
        public MeasurementFrequency Frequency { get; set; } = MeasurementFrequency.Monthly;

        /// <summary>
        /// Measurement direction indicating whether higher or lower values are better
        /// </summary>
        [Required]
        [Display(Name = "Measurement Direction")]
        public MeasurementDirection MeasurementDirection { get; set; } = MeasurementDirection.HigherIsBetter;

        /// <summary>
        /// Current performance trend
        /// </summary>
        [Display(Name = "Performance Trend")]
        public PerformanceTrend PerformanceTrend { get; set; } = PerformanceTrend.NotSet;

        /// <summary>
        /// Process area this Result Indicator belongs to
        /// </summary>
        [Display(Name = "Process Area")]
        public ProcessArea? ProcessArea { get; set; }

        /// <summary>
        /// Link to the Success Factor (SF) or Critical Success Factor (CSF)
        /// </summary>
        [Required]
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; set; }

        /// <summary>
        /// Navigation property to the Success Factor
        /// </summary>
        [ForeignKey("SuccessFactorId")]
        public virtual SuccessFactor SuccessFactor { get; set; } = null!;

        /// <summary>
        /// Person responsible for this indicator
        /// </summary>
        [Display(Name = "Responsible Person")]
        public string? ResponsiblePersonId { get; set; }

        /// <summary>
        /// Navigation property to the responsible person
        /// </summary>
        [ForeignKey("ResponsiblePersonId")]
        public virtual ApplicationUser? ResponsiblePerson { get; set; }

        /// <summary>
        /// Scope or boundary of what this indicator is measuring
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Measurement Scope")]
        public string? MeasurementScope { get; set; }

        /// <summary>
        /// The timeframe for which this result is relevant
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Time Frame")]
        public string? TimeFrame { get; set; }

        /// <summary>
        /// Primary source of data for this indicator
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Data Source")]
        public string? DataSource { get; set; }

        /// <summary>
        /// Type of result (e.g., Financial, Customer Satisfaction)
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Result Type")]
        public string? ResultType { get; set; }

        /// <summary>
        /// Current status of this indicator
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public IndicatorStatus Status { get; set; } = IndicatorStatus.Draft;

        /// <summary>
        /// Collection of related Performance Indicators that contribute to this Result Indicator
        /// </summary>
        public virtual ICollection<PerformanceIndicator>? PerformanceIndicators { get; set; }

        /// <summary>
        /// Collection of measurement values
        /// </summary>
        public virtual ICollection<Measurement>? Measurements { get; set; }

        /// <summary>
        /// Tần suất đo lường
        /// </summary>
        [Display(Name = "Tần suất đo lường")]
        public MeasurementFrequency MeasurementFrequency { get; set; } = MeasurementFrequency.Monthly;

        /// <summary>
        /// Thời gian cập nhật lần cuối
        /// </summary>
        [Display(Name = "Cập nhật lần cuối")]
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// ID phòng ban
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Tham chiếu đến phòng ban
        /// </summary>
        public virtual Department? Department { get; set; }
    }
}
