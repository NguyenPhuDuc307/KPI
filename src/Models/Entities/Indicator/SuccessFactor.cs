using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Indicator
{
    /// <summary>
    /// Represents a Success Factor (SF) - factors that contribute to achieving an objective.
    /// Combined entity for both SF and CSF (Critical Success Factors) with IsCritical flag.
    /// </summary>
    public class SuccessFactor : BaseEntity
    {
        /// <summary>
        /// Name of the Success Factor
        /// </summary>
        [Required]
        [StringLength(200)]
        [Display(Name = "Tên yếu tố thành công")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Success Factor
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        /// <summary>
        /// Unique code for identifying this SF/CSF
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Mã CSF")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if this is a Critical Success Factor (true) or a regular Success Factor (false)
        /// </summary>
        [Display(Name = "Yếu tố then chốt")]
        public bool IsCritical { get; set; }

        /// <summary>
        /// Priority of this SF/CSF (1-5, where 5 is highest)
        /// </summary>
        [Display(Name = "Mức độ ưu tiên")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        /// <summary>
        /// Weight for critical success factors (0-100) - only relevant when IsCritical is true
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Weight")]
        public int? Weight { get; set; }

        /// <summary>
        /// Current status of this SF/CSF
        /// </summary>
        [Display(Name = "Trạng thái")]
        public SuccessFactorStatus Status { get; set; } = SuccessFactorStatus.NotStarted;

        /// <summary>
        /// Parent Success Factor ID (for CSFs) - null for top-level SFs
        /// </summary>
        [Display(Name = "Parent Success Factor")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Navigation property to parent SF (for CSFs)
        /// </summary>
        [ForeignKey("ParentId")]
        public virtual SuccessFactor? Parent { get; set; }

        /// <summary>
        /// The business objective this SF supports - only for top-level SFs
        /// </summary>
        [Display(Name = "Business Objective")]
        public Guid? ObjectiveId { get; set; }

        /// <summary>
        /// Navigation property to the business objective
        /// </summary>
        [ForeignKey("ObjectiveId")]
        public virtual Objective? Objective { get; set; }

        /// <summary>
        /// Current progress percentage of this SF/CSF
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; } = 0;

        /// <summary>
        /// Start date when this SF/CSF becomes active
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Target date for achieving this SF/CSF
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; } = DateTime.UtcNow.AddYears(1);

        /// <summary>
        /// Department responsible for this SF/CSF
        /// </summary>
        [Display(Name = "Phòng ban")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Navigation property to the responsible department
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Person who owns or is responsible for this SF/CSF
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Người phụ trách")]
        public string? OwnerId { get; set; }

        /// <summary>
        /// User responsible for this SF/CSF
        /// </summary>
        [Display(Name = "Responsible User")]
        public string? ResponsibleUserId { get; set; }

        /// <summary>
        /// Navigation property to the responsible user
        /// </summary>
        [ForeignKey("ResponsibleUserId")]
        public virtual ApplicationUser? ResponsibleUser { get; set; }

        /// <summary>
        /// Risk level associated with this SF/CSF (only relevant for Critical Success Factors)
        /// </summary>
        [Display(Name = "Mức độ rủi ro")]
        public RiskLevel? RiskLevel { get; set; }

        /// <summary>
        /// Category of this SF/CSF (only relevant for Critical Success Factors)
        /// </summary>
        [Display(Name = "Phân loại")]
        public SuccessFactorCategory? Category { get; set; }

        /// <summary>
        /// Additional notes or comments about this SF/CSF
        /// </summary>
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Collection of child CSFs (when this is a parent SF)
        /// </summary>
        public virtual ICollection<SuccessFactor>? Children { get; set; }

        /// <summary>
        /// Collection of Result Indicators (RIs/KRIs) associated with this SF/CSF
        /// </summary>
        public virtual ICollection<ResultIndicator>? ResultIndicators { get; set; }

        /// <summary>
        /// Collection of Performance Indicators (PIs/KPIs) associated with this SF/CSF
        /// </summary>
        public virtual ICollection<PerformanceIndicator>? PerformanceIndicators { get; set; }

        /// <summary>
        /// Collection of measurement values
        /// </summary>
        public virtual ICollection<Measurement>? Measurements { get; set; }

        /// <summary>
        /// Last review date of this SF/CSF's progress
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Last Review Date")]
        public DateTime? LastReviewDate { get; set; }

        /// <summary>
        /// Next scheduled review date
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Next Review Date")]
        public DateTime? NextReviewDate { get; set; }

        /// <summary>
        /// Mục tiêu của yếu tố thành công
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá trị mục tiêu")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Giá trị hiện tại
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá trị hiện tại")]
        public decimal? CurrentValue { get; set; }

        /// <summary>
        /// Ngưỡng tối thiểu
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Ngưỡng tối thiểu")]
        public decimal? ThresholdValue { get; set; }

        /// <summary>
        /// Đơn vị đo lường
        /// </summary>
        [Display(Name = "Đơn vị")]
        [StringLength(50)]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Tần suất đo lường
        /// </summary>
        [Display(Name = "Tần suất đo lường")]
        public MeasurementFrequency MeasurementFrequency { get; set; } = MeasurementFrequency.Monthly;

        /// <summary>
        /// Hướng đo lường (cao hơn hay thấp hơn là tốt hơn)
        /// </summary>
        [Display(Name = "Hướng đo lường")]
        public MeasurementDirection Direction { get; set; } = MeasurementDirection.HigherIsBetter;

        /// <summary>
        /// Loại yếu tố thành công
        /// </summary>
        [Display(Name = "Loại yếu tố")]
        public SuccessFactorCategory FactorCategory { get; set; } = SuccessFactorCategory.Quality;

        /// <summary>
        /// Trạng thái của yếu tố thành công
        /// </summary>
        [Display(Name = "Trạng thái")]
        public SuccessFactorStatus FactorStatus { get; set; } = SuccessFactorStatus.NotStarted;

        /// <summary>
        /// Ngày cập nhật gần nhất
        /// </summary>
        [Display(Name = "Cập nhật gần nhất")]
        [DataType(DataType.DateTime)]
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Ngày đo lường gần nhất
        /// </summary>
        [Display(Name = "Đo lường gần nhất")]
        [DataType(DataType.Date)]
        public DateTime? LastMeasurementDate { get; set; }

        /// <summary>
        /// Ngày đo lường tiếp theo
        /// </summary>
        [Display(Name = "Đo lường tiếp theo")]
        [DataType(DataType.Date)]
        public DateTime? NextMeasurementDate { get; set; }

        /// <summary>
        /// Giá trị cơ sở (baseline) ban đầu
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá trị cơ sở")]
        public decimal? BaselineValue { get; set; }

        /// <summary>
        /// Ngưỡng trên - Vượt quá ngưỡng này được coi là tốt
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Ngưỡng trên")]
        public decimal? UpperThreshold { get; set; }

        /// <summary>
        /// Ngưỡng dưới - Dưới ngưỡng này được coi là kém
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Ngưỡng dưới")]
        public decimal? LowerThreshold { get; set; }

        /// <summary>
        /// Phương pháp tính toán/thu thập dữ liệu
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Phương pháp tính toán")]
        public string? CalculationMethod { get; set; }

        /// <summary>
        /// Nguồn dữ liệu
        /// </summary>
        [Display(Name = "Nguồn dữ liệu")]
        public DataSource? DataSource { get; set; }

        /// <summary>
        /// Tính phần trăm hoàn thành so với mục tiêu
        /// </summary>
        [NotMapped]
        [Display(Name = "% Hoàn thành")]
        public decimal? CompletionPercentage =>
            this.TargetValue != null && this.TargetValue != 0
            ? Math.Round((this.CurrentValue ?? 0) / this.TargetValue.Value * 100, 2)
            : 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public SuccessFactor()
        {
        }

        /// <summary>
        /// Constructor với các tham số cơ bản
        /// </summary>
        public SuccessFactor(string code, string name, string description, decimal targetValue,
            MeasurementUnit unit = MeasurementUnit.Number)
        {
            this.Code = code;
            this.Name = name;
            this.Description = description;
            this.TargetValue = targetValue;
            this.Unit = unit.ToString();
            this.Status = SuccessFactorStatus.NotStarted;
        }
    }
}
