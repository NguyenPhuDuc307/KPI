namespace KPISolution.Models.ViewModels.Department
{
    /// <summary>
    /// Hiển thị tổng quan về Indicator của một phòng ban
    /// </summary>
    public class DepartmentIndicatorOverviewViewModel
    {
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
        /// Tổng số Indicator của phòng ban
        /// </summary>
        [Display(Name = "Tổng số Indicator")]
        public int TotalIndicators { get; set; }

        /// <summary>
        /// Số lượng Indicator đang đạt mục tiêu
        /// </summary>
        [Display(Name = "Đang đạt mục tiêu")]
        public int OnTargetIndicators { get; set; }

        /// <summary>
        /// Số lượng Indicator đang ở mức rủi ro
        /// </summary>
        [Display(Name = "Đang có rủi ro")]
        public int AtRiskIndicators { get; set; }

        /// <summary>
        /// Số lượng Indicator dưới mục tiêu
        /// </summary>
        [Display(Name = "Dưới mục tiêu")]
        public int BelowTargetIndicators { get; set; }

        /// <summary>
        /// Hiệu suất trung bình của tất cả Indicator (%)
        /// </summary>
        [Display(Name = "Hiệu suất trung bình")]
        public decimal AveragePerformance { get; set; }

        /// <summary>
        /// Tỷ lệ hoàn thành Indicator (%)
        /// </summary>
        [Display(Name = "Tỷ lệ hoàn thành")]
        public decimal CompletionRate { get; set; }

        /// <summary>
        /// Danh sách các phép đo sắp tới cần thực hiện
        /// </summary>
        [Display(Name = "Phép đo sắp tới")]
        public List<IndicatorViewModel> UpcomingMeasurements { get; set; }

        /// <summary>
        /// Danh sách các phép đo quá hạn
        /// </summary>
        [Display(Name = "Phép đo quá hạn")]
        public List<IndicatorViewModel> OverdueMeasurements { get; set; }

        /// <summary>
        /// Các giá trị Indicator gần đây
        /// </summary>
        [Display(Name = "Giá trị gần đây")]
        public List<KpiValueViewModel> RecentMeasurements { get; set; }

        /// <summary>
        /// Danh sách các Indicator có hiệu suất cao nhất
        /// </summary>
        [Display(Name = "Hiệu suất cao nhất")]
        public List<IndicatorViewModel> TopPerformingIndicators { get; set; }

        /// <summary>
        /// Danh sách các Indicator có nguy cơ không đạt mục tiêu
        /// </summary>
        [Display(Name = "Indicator có nguy cơ")]
        public List<IndicatorViewModel> IndicatorsAtRisk { get; set; }

        /// <summary>
        /// Khởi tạo với giá trị mặc định
        /// </summary>
        public DepartmentIndicatorOverviewViewModel()
        {
            this.UpcomingMeasurements = [];
            this.OverdueMeasurements = [];
            this.RecentMeasurements = [];
            this.TopPerformingIndicators = [];
            this.IndicatorsAtRisk = [];
        }
    }

    /// <summary>
    /// Hiển thị giá trị của một Indicator
    /// </summary>
    public class KpiValueViewModel
    {
        /// <summary>
        /// ID của Indicator
        /// </summary>
        public Guid IndicatorId { get; set; }

        /// <summary>
        /// Tên của Indicator
        /// </summary>
        public string IndicatorName { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị đo được
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Giá trị mục tiêu
        /// </summary>
        public decimal Target { get; set; }

        /// <summary>
        /// Thời gian đo
        /// </summary>
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Phần trăm so với mục tiêu
        /// </summary>
        public decimal PercentageOfTarget => this.Target == 0 ? 0 : Math.Round((this.Value / this.Target) * 100, 1);
    }
}
