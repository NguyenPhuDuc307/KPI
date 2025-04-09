namespace KPISolution.Models.ViewModels.Department
{
    /// <summary>
    /// Hiển thị thông tin tổng quan về một phòng ban trong dashboard
    /// </summary>
    public class DepartmentDashboardViewModel
    {
        /// <summary>
        /// Khởi tạo mặc định
        /// </summary>
        public DepartmentDashboardViewModel()
        {
            this.KpiSummaries = [];
            this.LinkedSuccessFactors = [];
            this.PerformanceTrend = new Dictionary<string, decimal>();
        }

        /// <summary>
        /// ID của phòng ban
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        [Display(Name = "Phòng ban")]
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách tóm tắt các Indicator của phòng ban
        /// </summary>
        [Display(Name = "Indicators")]
        public List<IndicatorSummaryViewModel> KpiSummaries { get; set; } = [];

        /// <summary>
        /// Danh sách các Success Factor liên quan đến phòng ban
        /// </summary>
        [Display(Name = "Critical Success Factors")]
        public List<SuccessFactorSummaryViewModel> LinkedSuccessFactors { get; set; } = [];

        /// <summary>
        /// Tổng số Indicator
        /// </summary>
        [Display(Name = "Tổng số Indicator")]
        public int TotalIndicatorCount { get; set; }

        /// <summary>
        /// Số lượng Indicator có nguy cơ
        /// </summary>
        [Display(Name = "Indicators có nguy cơ")]
        public int AtRiskIndicatorCount { get; set; }

        /// <summary>
        /// Phần trăm Indicator đạt mục tiêu
        /// </summary>
        [Display(Name = "% đạt mục tiêu")]
        public decimal OnTargetPercentage { get; set; }

        /// <summary>
        /// Hiệu suất tổng thể của phòng ban (0-100)
        /// </summary>
        [Display(Name = "Hiệu suất")]
        public decimal OverallPerformance { get; set; }

        /// <summary>
        /// CSS class hiển thị hiệu suất
        /// </summary>
        public string PerformanceCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Thời điểm cập nhật dashboard
        /// </summary>
        [Display(Name = "Cập nhật lúc")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thông tin xu hướng hiệu suất theo thời gian
        /// </summary>
        [Display(Name = "Xu hướng hiệu suất")]
        public Dictionary<string, decimal> PerformanceTrend { get; set; }

        /// <summary>
        /// Tên trưởng phòng
        /// </summary>
        [Display(Name = "Trưởng phòng")]
        public string DepartmentHead { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng nhân viên trong phòng ban
        /// </summary>
        [Display(Name = "Số nhân viên")]
        public int EmployeeCount { get; set; }
    }
}
