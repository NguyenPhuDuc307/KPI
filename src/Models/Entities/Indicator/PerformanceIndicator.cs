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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Performance Indicator
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
        /// Indicates if this is a Key Performance Indicator (true) or a regular Performance Indicator (false)
        /// </summary>
        [Display(Name = "Is Key Performance Indicator")]
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
        /// Parent Result Indicator that this Performance Indicator contributes to
        /// </summary>
        [Display(Name = "Result Indicator")]
        public Guid? ResultIndicatorId { get; set; }

        /// <summary>
        /// Navigation property to the Result Indicator
        /// </summary>
        [ForeignKey("ResultIndicatorId")]
        public virtual ResultIndicator? ResultIndicator { get; set; }

        /// <summary>
        /// Type of activity being measured (operational, strategic, etc.)
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Activity Type")]
        public string? ActivityType { get; set; }

        /// <summary>
        /// The level of performance (standard, exceptional, etc.)
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Performance Level")]
        public string? PerformanceLevel { get; set; }

        /// <summary>
        /// The control level this indicator belongs to (operational, tactical, strategic)
        /// </summary>
        [Display(Name = "Control Level")]
        public ControlLevel? ControlLevel { get; set; }

        /// <summary>
        /// The type of indicator (PI, KPI)
        /// </summary>
        [Required]
        [Display(Name = "Indicator Type")]
        public IndicatorType IndicatorType { get; set; } = IndicatorType.PI;

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
        /// Department associated with this indicator
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Navigation property to the department
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Team member responsible for implementing actions
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Responsible Team Member")]
        public string? ResponsibleTeamMember { get; set; }

        /// <summary>
        /// Frequency of reviewing this indicator
        /// </summary>
        [Display(Name = "Review Frequency")]
        public MeasurementFrequency? ReviewFrequency { get; set; }

        /// <summary>
        /// Ngưỡng cảnh báo thấp - kích hoạt cảnh báo khi giá trị thấp hơn ngưỡng này
        /// </summary>
        [Display(Name = "Ngưỡng cảnh báo thấp")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? MinAlertThreshold { get; set; }

        /// <summary>
        /// Ngưỡng cảnh báo cao - kích hoạt cảnh báo khi giá trị cao hơn ngưỡng này
        /// </summary>
        [Display(Name = "Ngưỡng cảnh báo cao")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? MaxAlertThreshold { get; set; }

        /// <summary>
        /// Method used to collect data for this indicator
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Data Collection Method")]
        public string? DataCollectionMethod { get; set; }

        /// <summary>
        /// Action plan for improving performance on this indicator
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Action Plan")]
        public string? ActionPlan { get; set; }

        /// <summary>
        /// Current status of this indicator
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public IndicatorStatus Status { get; set; } = IndicatorStatus.Draft;

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
        /// Success Factor liên kết trực tiếp với Performance Indicator
        /// </summary>
        [Display(Name = "Success Factor")]
        public Guid? SuccessFactorId { get; set; }

        /// <summary>
        /// Navigation property to the Success Factor
        /// </summary>
        [ForeignKey("SuccessFactorId")]
        public virtual SuccessFactor? SuccessFactor { get; set; }
    }
}
