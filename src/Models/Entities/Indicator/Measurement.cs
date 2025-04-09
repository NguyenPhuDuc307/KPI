using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Indicator
{
    /// <summary>
    /// Represents a measurement value recorded for an indicator at a specific point in time.
    /// This unified entity can be associated with any type of indicator (SuccessFactor, ResultIndicator, or PerformanceIndicator).
    /// </summary>
    public class Measurement : BaseEntity
    {
        /// <summary>
        /// ID của PerformanceIndicator (nếu đây là phép đo của chỉ số hiệu suất)
        /// </summary>
        public Guid? PerformanceIndicatorId { get; set; }

        /// <summary>
        /// Navigation property đến PerformanceIndicator
        /// </summary>
        [ForeignKey("PerformanceIndicatorId")]
        public virtual PerformanceIndicator? PerformanceIndicator { get; set; }

        /// <summary>
        /// ID của ResultIndicator (nếu đây là phép đo của chỉ số kết quả)
        /// </summary>
        public Guid? ResultIndicatorId { get; set; }

        /// <summary>
        /// Navigation property đến ResultIndicator
        /// </summary>
        [ForeignKey("ResultIndicatorId")]
        public virtual ResultIndicator? ResultIndicator { get; set; }

        /// <summary>
        /// ID của SuccessFactor (nếu đây là phép đo của yếu tố thành công)
        /// </summary>
        public Guid? SuccessFactorId { get; set; }

        /// <summary>
        /// Navigation property đến SuccessFactor
        /// </summary>
        [ForeignKey("SuccessFactorId")]
        public virtual SuccessFactor? SuccessFactor { get; set; }

        /// <summary>
        /// Giá trị đo được
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá trị")]
        public decimal Value { get; set; }

        /// <summary>
        /// Ngày thực hiện phép đo
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày đo")]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// Kỳ đo lường (ví dụ: "Q1 2023", "Tháng 3/2023")
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Kỳ đo lường")]
        public string? Period { get; set; }

        /// <summary>
        /// Trạng thái của phép đo
        /// </summary>
        [Display(Name = "Trạng thái")]
        public MeasurementStatus Status { get; set; } = MeasurementStatus.Actual;

        /// <summary>
        /// Ghi chú về phép đo
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        /// <summary>
        /// Người thực hiện phép đo
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Người đo")]
        public string? MeasuredBy { get; set; }

        /// <summary>
        /// Phương pháp thu thập dữ liệu
        /// </summary>
        [Display(Name = "Phương pháp thu thập")]
        public DataCollectionMethod? CollectionMethod { get; set; }

        /// <summary>
        /// Nguồn dữ liệu
        /// </summary>
        [Display(Name = "Nguồn dữ liệu")]
        public DataSource? Source { get; set; }

        /// <summary>
        /// The type of measurement (distinguishes between different indicator types)
        /// </summary>
        [Display(Name = "Loại chỉ số")]
        public IndicatorMeasurementType IndicatorType { get; set; }

        /// <summary>
        /// The unit of measurement
        /// </summary>
        [Display(Name = "Đơn vị")]
        public MeasurementUnit Unit { get; set; } = MeasurementUnit.Number;

        /// <summary>
        /// Person who submitted this measurement
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Người cập nhật")]
        public string? SubmittedById { get; set; }

        /// <summary>
        /// Navigation property to the user who submitted this measurement
        /// </summary>
        [ForeignKey("SubmittedById")]
        public virtual ApplicationUser? SubmittedBy { get; set; }

        /// <summary>
        /// Kiểm tra xem phép đo này có thuộc về bất kỳ chỉ số nào hay không
        /// </summary>
        [NotMapped]
        public bool HasIndicator => this.PerformanceIndicatorId.HasValue || this.ResultIndicatorId.HasValue || this.SuccessFactorId.HasValue;

        /// <summary>
        /// Loại chỉ số được đo lường
        /// </summary>
        public IndicatorMeasurementType GetIndicatorType()
        {
            if (this.PerformanceIndicatorId.HasValue)
                return IndicatorMeasurementType.PerformanceIndicator;
            if (this.ResultIndicatorId.HasValue)
                return IndicatorMeasurementType.ResultIndicator;
            if (this.SuccessFactorId.HasValue)
                return IndicatorMeasurementType.SuccessFactor;

            return IndicatorMeasurementType.Unknown;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Measurement()
        {
        }

        /// <summary>
        /// Constructor với các tham số cơ bản
        /// </summary>
        /// <param name="value">Giá trị đo được</param>
        /// <param name="date">Ngày đo</param>
        public Measurement(decimal value, DateTime date)
        {
            this.Value = value;
            this.MeasurementDate = date;
            this.Status = MeasurementStatus.Actual;
        }

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

            throw new InvalidOperationException("Measurement must be associated with an indicator");
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
    }
}
