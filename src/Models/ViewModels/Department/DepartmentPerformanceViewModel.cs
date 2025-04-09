namespace KPISolution.Models.ViewModels.Department
{
    /// <summary>
    /// Hiển thị thông tin hiệu suất tổng thể của phòng ban
    /// </summary>
    public class DepartmentPerformanceViewModel
    {
        /// <summary>
        /// ID của phòng ban
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        [Display(Name = "Phòng ban")]
        public string DepartmentName { get; set; }

        /// <summary>
        /// Điểm hiệu suất tổng thể (0-100)
        /// </summary>
        [Display(Name = "Điểm hiệu suất")]
        public decimal PerformanceScore { get; set; }

        /// <summary>
        /// Tỷ lệ đạt mục tiêu (%)
        /// </summary>
        [Display(Name = "Tỷ lệ đạt mục tiêu")]
        public decimal TargetAchievementRate { get; set; }

        /// <summary>
        /// Tỷ lệ hoàn thành (%)
        /// </summary>
        [Display(Name = "Tỷ lệ hoàn thành")]
        public decimal CompletionRate { get; set; }

        /// <summary>
        /// Điểm đạt được theo CSF
        /// </summary>
        [Display(Name = "Điểm CSF")]
        public decimal CsfScore { get; set; }

        /// <summary>
        /// Điểm chất lượng dữ liệu (0-100)
        /// </summary>
        [Display(Name = "Chất lượng dữ liệu")]
        public decimal DataQualityScore { get; set; }

        /// <summary>
        /// Tổng số Indicator
        /// </summary>
        [Display(Name = "Tổng số Indicator")]
        public int TotalIndicators { get; set; }

        /// <summary>
        /// Số Indicator đang đạt mục tiêu
        /// </summary>
        [Display(Name = "Indicator đạt mục tiêu")]
        public int IndicatorsOnTarget { get; set; }

        /// <summary>
        /// Số Indicator dưới mục tiêu
        /// </summary>
        [Display(Name = "Indicator dưới mục tiêu")]
        public int IndicatorsBelowTarget { get; set; }

        /// <summary>
        /// Thời gian đo lường gần nhất
        /// </summary>
        [Display(Name = "Cập nhật gần nhất")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Danh sách các Indicator hoạt động tốt nhất
        /// </summary>
        [Display(Name = "Indicator hoạt động tốt nhất")]
        public List<IndicatorViewModel> TopPerformingIndicators { get; set; }

        /// <summary>
        /// Danh sách các Indicator hoạt động kém
        /// </summary>
        [Display(Name = "Indicator cần cải thiện")]
        public List<IndicatorViewModel> UnderperformingIndicators { get; set; }

        /// <summary>
        /// Hiệu suất theo thời gian
        /// </summary>
        [Display(Name = "Hiệu suất theo thời gian")]
        public Dictionary<string, decimal> PerformanceByPeriod { get; set; }

        /// <summary>
        /// CSS class cho hiển thị hiệu suất
        /// </summary>
        [Display(Name = "CSS Class")]
        public string PerformanceCssClass { get; set; }

        /// <summary>
        /// Phần trăm hiệu suất
        /// </summary>
        [Display(Name = "Phần trăm hiệu suất")]
        public int PerformancePercentage { get; set; }

        /// <summary>
        /// Số lượng Indicator
        /// </summary>
        [Display(Name = "Số lượng Indicator")]
        public int IndicatorCount { get; set; }

        /// <summary>
        /// Số lượng Indicator cần chú ý
        /// </summary>
        [Display(Name = "Số Indicator cần chú ý")]
        public int AtRiskCount { get; set; }

        /// <summary>
        /// Tên phòng ban/đơn vị
        /// </summary>
        [Display(Name = "Tên")]
        public string Name => DepartmentName;

        /// <summary>
        /// Khởi tạo với giá trị mặc định
        /// </summary>
        public DepartmentPerformanceViewModel()
        {
            this.DepartmentName = string.Empty;
            this.PerformanceByPeriod = new Dictionary<string, decimal>();
            this.TopPerformingIndicators = [];
            this.UnderperformingIndicators = [];
            this.LastUpdated = DateTime.UtcNow;
            this.PerformanceCssClass = string.Empty;
        }
    }
}
