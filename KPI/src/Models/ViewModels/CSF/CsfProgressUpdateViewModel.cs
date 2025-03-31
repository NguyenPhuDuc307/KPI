using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.CSF
{
    /// <summary>
    /// View model for creating or updating a CSF progress entry
    /// </summary>
    public class CsfProgressUpdateViewModel
    {
        /// <summary>
        /// CSF ID
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// ID of the CSF this progress update is for
        /// </summary>
        public Guid CSFId { get; set; }

        /// <summary>
        /// Name of the CSF this progress update is for
        /// </summary>
        public string CSFName { get; set; } = string.Empty;

        /// <summary>
        /// CSF code
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// CSF name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Department name
        /// </summary>
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// CSF owner
        /// </summary>
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// Current progress percentage before update
        /// </summary>
        public int CurrentProgressPercentage { get; set; }

        /// <summary>
        /// Current status before update
        /// </summary>
        public CSFStatus CurrentStatus { get; set; }

        /// <summary>
        /// Current risk level before update
        /// </summary>
        public RiskLevel CurrentRiskLevel { get; set; }

        /// <summary>
        /// New progress percentage (0-100)
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// New status after update
        /// </summary>
        [Display(Name = "Trạng thái")]
        public CSFStatus Status { get; set; }

        /// <summary>
        /// New risk level after update
        /// </summary>
        [Display(Name = "Mức độ rủi ro")]
        public RiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Update date
        /// </summary>
        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Recent achievements
        /// </summary>
        [Display(Name = "Kết quả đạt được")]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Achievements { get; set; } = string.Empty;

        /// <summary>
        /// Challenges encountered
        /// </summary>
        [Display(Name = "Thách thức & khó khăn")]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Challenges { get; set; } = string.Empty;

        /// <summary>
        /// Next steps
        /// </summary>
        [Display(Name = "Các bước tiếp theo")]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string NextSteps { get; set; } = string.Empty;

        /// <summary>
        /// Whether the CSF needs special attention
        /// </summary>
        [Display(Name = "Cần chú ý")]
        public bool NeedsAttention { get; set; }

        /// <summary>
        /// Target completion date
        /// </summary>
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Expected completion date based on current progress
        /// </summary>
        [Display(Name = "Dự kiến hoàn thành")]
        public DateTime? ExpectedCompletionDate { get; set; }

        /// <summary>
        /// Next review date
        /// </summary>
        [Display(Name = "Ngày đánh giá tiếp theo")]
        public DateTime? NextReviewDate { get; set; }

        /// <summary>
        /// Days remaining to target date
        /// </summary>
        public int DaysRemaining { get; set; }

        /// <summary>
        /// Percentage of time elapsed
        /// </summary>
        [Display(Name = "Thời gian đã qua (%)")]
        public int TimeElapsedPercentage { get; set; }

        /// <summary>
        /// Flag indicating if progress is on track compared to elapsed time
        /// </summary>
        [Display(Name = "Đúng tiến độ")]
        public bool IsOnTrack { get; set; }

        /// <summary>
        /// Current status
        /// </summary>
        public string StatusDisplay { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for status
        /// </summary>
        public string StatusCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Status options for dropdown
        /// </summary>
        public SelectList? StatusOptions { get; set; }

        /// <summary>
        /// Risk level options for dropdown
        /// </summary>
        public SelectList? RiskLevelOptions { get; set; }
    }
}
