using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Progress
{
    /// <summary>
    /// Đại diện cho một bản ghi cập nhật tiến độ của Success Factor
    /// </summary>
    public class ProgressUpdate : BaseEntity
    {
        /// <summary>
        /// ID của Success Factor được cập nhật
        /// </summary>
        [Required]
        [Display(Name = "Success Factor")]
        public Guid SuccessFactorId { get; set; }

        /// <summary>
        /// Navigation property đến Success Factor
        /// </summary>
        [ForeignKey("SuccessFactorId")]
        public virtual SuccessFactor? SuccessFactor { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        [Required]
        [Display(Name = "Ngày cập nhật")]
        [DataType(DataType.Date)]
        public DateTime UpdateDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Phần trăm tiến độ hiện tại
        /// </summary>
        [Required]
        [Range(0, 100)]
        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Phần trăm tiến độ trước đó, để so sánh
        /// </summary>
        [Display(Name = "Tiến độ trước (%)")]
        public int? PreviousPercentage { get; set; }

        /// <summary>
        /// Trạng thái hiện tại
        /// </summary>
        [Required]
        [Display(Name = "Trạng thái")]
        public SuccessFactorStatus Status { get; set; }

        /// <summary>
        /// Trạng thái trước đó, để so sánh
        /// </summary>
        [Display(Name = "Trạng thái trước")]
        public SuccessFactorStatus? PreviousStatus { get; set; }

        /// <summary>
        /// Mức độ rủi ro
        /// </summary>
        [Required]
        [Display(Name = "Mức độ rủi ro")]
        public RiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Mức độ rủi ro trước đó, để so sánh
        /// </summary>
        [Display(Name = "Mức độ rủi ro trước")]
        public RiskLevel? PreviousRiskLevel { get; set; }

        /// <summary>
        /// Nhận xét hoặc ghi chú về cập nhật
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Nhận xét")]
        public string? Comments { get; set; }

        /// <summary>
        /// Vấn đề gặp phải
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Vấn đề")]
        public string? Issues { get; set; }

        /// <summary>
        /// Hành động đã thực hiện
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Hành động")]
        public string? Actions { get; set; }

        /// <summary>
        /// Các bước tiếp theo
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Bước tiếp theo")]
        public string? NextSteps { get; set; }

        /// <summary>
        /// Ngày cập nhật tiếp theo
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Cập nhật tiếp theo")]
        public DateTime? NextUpdateDate { get; set; }

        /// <summary>
        /// Có cần sự chú ý của quản lý không
        /// </summary>
        [Display(Name = "Cần chú ý")]
        public bool NeedsAttention { get; set; }

        /// <summary>
        /// Lý do cần sự chú ý
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Lý do cần chú ý")]
        public string? AttentionReason { get; set; }

        /// <summary>
        /// Kết quả đạt được trong kỳ này
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Kết quả đạt được")]
        public string? Achievements { get; set; }

        /// <summary>
        /// Có đúng tiến độ không
        /// </summary>
        [Display(Name = "Đúng tiến độ")]
        public bool IsOnTrack { get; set; }
    }
}