using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Measurement
{
    /// <summary>
    /// Mục tiêu cho các chỉ số
    /// </summary>
    public class Target : BaseEntity
    {
        /// <summary>
        /// Loại chỉ số
        /// </summary>
        [Display(Name = "Loại chỉ số")]
        public IndicatorMeasurementType IndicatorType { get; init; }

        /// <summary>
        /// ID của chỉ số
        /// </summary>
        [Display(Name = "Chỉ số")]
        public Guid IndicatorId { get; init; }

        /// <summary>
        /// Giá trị mục tiêu
        /// </summary>
        [Display(Name = "Giá trị mục tiêu")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetValue { get; init; }

        /// <summary>
        /// Giá trị tối thiểu
        /// </summary>
        [Display(Name = "Ngưỡng tối thiểu")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumValue { get; init; }

        /// <summary>
        /// Giá trị tối đa
        /// </summary>
        [Display(Name = "Ngưỡng tối đa")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaximumValue { get; init; }

        /// <summary>
        /// Kỳ áp dụng
        /// </summary>
        [Display(Name = "Kỳ áp dụng")]
        [StringLength(50)]
        public string Period { get; init; } = string.Empty;

        /// <summary>
        /// Ngày bắt đầu áp dụng
        /// </summary>
        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; init; }

        /// <summary>
        /// Ngày kết thúc áp dụng
        /// </summary>
        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; init; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? Notes { get; init; }

        /// <summary>
        /// Description or rationale for this target
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Description")]
        public string? Description { get; init; }

        /// <summary>
        /// Type of target (Stretch, Committed, Minimum)
        /// </summary>
        [StringLength(20)]
        [Display(Name = "Target Type")]
        public string? TargetType { get; init; } = "Committed";

        /// <summary>
        /// Person who approved this target
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Approved By")]
        public string? ApprovedBy { get; init; }

        /// <summary>
        /// Date when this target was approved
        /// </summary>
        [Display(Name = "Approval Date")]
        [DataType(DataType.Date)]
        public DateTime? ApprovalDate { get; init; }

        /// <summary>
        /// The stretch target (beyond normal target)
        /// </summary>
        [Display(Name = "Stretch Target")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? StretchTarget { get; init; }

        /// <summary>
        /// Current status of this target (Pending, Active, Achieved, Missed)
        /// </summary>
        [Display(Name = "Status")]
        public TargetStatus Status { get; init; } = TargetStatus.Pending;

        /// <summary>
        /// Unit of measurement for this target
        /// </summary>
        [Display(Name = "Unit")]
        public MeasurementUnit Unit { get; init; } = MeasurementUnit.Number;

        /// <summary>
        /// Person who created this target
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Created By")]
        public string? CreatedById { get; init; }

        /// <summary>
        /// Navigation property to the user who created this target
        /// </summary>
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser? CreatedByUser { get; init; }

        #region IndicatorRelationships

        /// <summary>
        /// ID of the Success Factor this target belongs to (optional)
        /// </summary>
        [Display(Name = "Success Factor")]
        public Guid? SuccessFactorId { get; init; }

        /// <summary>
        /// Navigation property to the Success Factor
        /// </summary>
        [ForeignKey("SuccessFactorId")]
        public virtual SuccessFactor? SuccessFactor { get; init; }

        /// <summary>
        /// ID of the Result Indicator this target belongs to (optional)
        /// </summary>
        [Display(Name = "Result Indicator")]
        public Guid? ResultIndicatorId { get; init; }

        /// <summary>
        /// Navigation property to the Result Indicator
        /// </summary>
        [ForeignKey("ResultIndicatorId")]
        public virtual ResultIndicator? ResultIndicator { get; init; }

        /// <summary>
        /// ID of the Performance Indicator this target belongs to (optional)
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

            throw new InvalidOperationException("Target must be associated with an indicator");
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
        /// Gets whether the associated indicator is a key indicator (KPI, KRI, or CSF)
        /// </summary>
        /// <returns>True if the associated indicator is a key indicator</returns>
        public bool? IsKeyIndicator()
        {
            if (this.SuccessFactor != null)
                return this.SuccessFactor.IsCritical;

            if (this.ResultIndicator != null)
                return this.ResultIndicator.IsKey;

            if (this.PerformanceIndicator != null)
                return this.PerformanceIndicator.IsKey;

            return null;
        }

        #endregion
    }
}
