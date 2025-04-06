using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Indicator
{
    /// <summary>
    /// Represents a Performance Indicator - measures how well an activity is performed.
    /// Combined entity for both PI and KPI (Key Performance Indicators) with IsKey flag.
    /// </summary>
    public class PerformanceIndicator : BaseEntity
    {
        /// <summary>
        /// Name of the Performance Indicator
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Description of the Performance Indicator
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
        /// Indicates if this is a Key Performance Indicator (true) or a regular Performance Indicator (false)
        /// </summary>
        [Display(Name = "Is Key Performance Indicator")]
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
        /// Parent Result Indicator that this Performance Indicator contributes to
        /// </summary>
        [Display(Name = "Result Indicator")]
        public Guid? ResultIndicatorId { get; init; }

        /// <summary>
        /// Navigation property to the Result Indicator
        /// </summary>
        [ForeignKey("ResultIndicatorId")]
        public virtual ResultIndicator? ResultIndicator { get; init; }

        /// <summary>
        /// Type of activity being measured (operational, strategic, etc.)
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Activity Type")]
        public string? ActivityType { get; init; }

        /// <summary>
        /// The level of performance (standard, exceptional, etc.)
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Performance Level")]
        public string? PerformanceLevel { get; init; }

        /// <summary>
        /// The control level this indicator belongs to (operational, tactical, strategic)
        /// </summary>
        [Display(Name = "Control Level")]
        public ControlLevel? ControlLevel { get; init; }

        /// <summary>
        /// The type of indicator (PI, KPI)
        /// </summary>
        [Required]
        [Display(Name = "Indicator Type")]
        public IndicatorType IndicatorType { get; init; } = IndicatorType.PI;

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
        /// Department associated with this indicator
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; init; }

        /// <summary>
        /// Navigation property to the department
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; init; }

        /// <summary>
        /// Team member responsible for implementing actions
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Responsible Team Member")]
        public string? ResponsibleTeamMember { get; init; }

        /// <summary>
        /// Frequency of reviewing this indicator
        /// </summary>
        [Display(Name = "Review Frequency")]
        public MeasurementFrequency? ReviewFrequency { get; init; }

        /// <summary>
        /// Threshold at which alerts should be triggered
        /// </summary>
        [Display(Name = "Alert Threshold")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? AlertThreshold { get; init; }

        /// <summary>
        /// Method used to collect data for this indicator
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Data Collection Method")]
        public string? DataCollectionMethod { get; init; }

        /// <summary>
        /// Action plan for improving performance on this indicator
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Action Plan")]
        public string? ActionPlan { get; init; }

        /// <summary>
        /// Current status of this indicator
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public IndicatorStatus Status { get; init; } = IndicatorStatus.Draft;

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
    }
}
