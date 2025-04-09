namespace KPISolution.Models.ViewModels.SuccessFactor
{
    /// <summary>
    /// View model cho việc hiển thị một yếu tố thành công trong danh sách
    /// </summary>
    public class SuccessFactorListItemViewModel
    {
        /// <summary>
        /// ID của yếu tố thành công
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Mã yếu tố thành công
        /// </summary>
        [Display(Name = "Mã")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Tên yếu tố thành công
        /// </summary>
        [Display(Name = "Tên")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả ngắn gọn
        /// </summary>
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Tỷ lệ hoàn thành (%)
        /// </summary>
        [Display(Name = "% Hoàn thành")]
        public decimal ProgressPercentage { get; set; }

        /// <summary>
        /// Trạng thái của yếu tố thành công
        /// </summary>
        [Display(Name = "Trạng thái")]
        public SuccessFactorStatus Status { get; set; }

        /// <summary>
        /// Mức độ rủi ro
        /// </summary>
        [Display(Name = "Mức độ rủi ro")]
        public RiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Có phải là yếu tố cốt lõi không
        /// </summary>
        [Display(Name = "Critical Success Factor")]
        public bool IsCritical { get; set; }

        /// <summary>
        /// Mức độ ưu tiên
        /// </summary>
        [Display(Name = "Ưu tiên")]
        public PriorityLevel Priority { get; set; }

        /// <summary>
        /// Danh mục
        /// </summary>
        [Display(Name = "Danh mục")]
        public SuccessFactorCategory Category { get; set; }

        /// <summary>
        /// ID phòng ban chịu trách nhiệm
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Tên phòng ban chịu trách nhiệm
        /// </summary>
        [Display(Name = "Phòng ban")]
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// ID của mục tiêu liên quan
        /// </summary>
        public Guid? ObjectiveId { get; set; }

        /// <summary>
        /// Tên của mục tiêu liên quan
        /// </summary>
        [Display(Name = "Mục tiêu")]
        public string? ObjectiveName { get; set; }

        /// <summary>
        /// Số lượng chỉ số (KPI) liên kết
        /// </summary>
        [Display(Name = "Số chỉ số")]
        public int IndicatorCount { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        [Display(Name = "Cập nhật")]
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Get CSS class for the status badge
        /// </summary>
        public string StatusBadgeClass
        {
            get
            {
                return this.Status switch
                {
                    SuccessFactorStatus.NotStarted => "badge bg-secondary",
                    SuccessFactorStatus.InProgress => "badge bg-primary",
                    SuccessFactorStatus.AtRisk => "badge bg-warning text-dark",
                    SuccessFactorStatus.Completed => "badge bg-success",
                    SuccessFactorStatus.Cancelled => "badge bg-danger",
                    SuccessFactorStatus.OffTrack => "badge bg-info text-dark",
                    _ => "badge bg-secondary"
                };
            }
        }

        /// <summary>
        /// Get CSS class for the progress bar
        /// </summary>
        public string ProgressBarClass
        {
            get
            {
                return this.ProgressPercentage switch
                {
                    100 => "progress-bar bg-success",
                    >= 75 => "progress-bar bg-info",
                    >= 50 => "progress-bar bg-primary",
                    >= 25 => "progress-bar bg-warning",
                    _ => "progress-bar bg-danger"
                };
            }
        }

        /// <summary>
        /// Get CSS class for the priority badge
        /// </summary>
        public string PriorityBadgeClass
        {
            get
            {
                return this.Priority switch
                {
                    PriorityLevel.Low => "badge bg-success",
                    PriorityLevel.Medium => "badge bg-info",
                    PriorityLevel.High => "badge bg-warning text-dark",
                    PriorityLevel.Critical => "badge bg-danger",
                    _ => "badge bg-secondary"
                };
            }
        }

        /// <summary>
        /// Get CSS class for the risk level badge
        /// </summary>
        public string RiskBadgeClass
        {
            get
            {
                return this.RiskLevel switch
                {
                    RiskLevel.None => "badge bg-light text-dark",
                    RiskLevel.Low => "badge bg-success",
                    RiskLevel.Medium => "badge bg-info",
                    RiskLevel.High => "badge bg-warning text-dark",
                    RiskLevel.Critical => "badge bg-danger",
                    RiskLevel.Negligible => "badge bg-light text-dark",
                    _ => "badge bg-secondary"
                };
            }
        }

        /// <summary>
        /// Gets a badge class based on the priority.
        /// </summary>
        public string GetPriorityBadgeClass()
        {
            return this.Priority switch
            {
                PriorityLevel.Low => "bg-info",
                PriorityLevel.Medium => "bg-primary",
                PriorityLevel.High => "bg-warning",
                PriorityLevel.Critical => "bg-danger",
                _ => "bg-secondary",
            };
        }

        /// <summary>
        /// Gets a badge class based on the risk level.
        /// </summary>
        public string GetRiskLevelBadgeClass()
        {
            return this.RiskLevel switch
            {
                RiskLevel.None => "bg-success",
                RiskLevel.Negligible => "bg-info",
                RiskLevel.Low => "bg-info",
                RiskLevel.Medium => "bg-primary",
                RiskLevel.High => "bg-warning",
                RiskLevel.Critical => "bg-danger",
                _ => "bg-secondary",
            };
        }

        /// <summary>
        /// Gets a badge class based on the category.
        /// </summary>
        public string GetCategoryBadgeClass()
        {
            return this.Category switch
            {
                SuccessFactorCategory.Financial => "bg-success",
                SuccessFactorCategory.Customer => "bg-info",
                SuccessFactorCategory.InternalProcess => "bg-primary",
                SuccessFactorCategory.LearningAndGrowth => "bg-warning",
                SuccessFactorCategory.Other => "bg-secondary",
                _ => "bg-secondary",
            };
        }

        /// <summary>
        /// Gets an indicator status based on the critical flag.
        /// </summary>
        public string GetCriticalBadgeClass()
        {
            return this.IsCritical ? "bg-danger" : "bg-secondary";
        }
    }
}
