namespace KPISolution.Models.ViewModels.Measurement
{
    public class MeasurementReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Guid? DepartmentId { get; set; }
        public List<SelectListItem> Departments { get; set; } = new();
        public List<IndicatorValueViewModel> Items { get; set; } = new();

        // Thống kê tổng quan
        public int TotalMeasurements => this.Items.Count;
        public int OnTargetCount => this.Items.Count(x => x.AchievementPercentage >= 100);
        public int BelowTargetCount => this.Items.Count(x => x.AchievementPercentage < 100);
        public decimal AverageAchievement => this.Items.Any() ? this.Items.Average(x => x.AchievementPercentage) : 0;

        // Phân loại theo loại chỉ số
        public int KPICount => this.Items.Count(x => x.IndicatorCode.StartsWith("KPI"));
        public int PICount => this.Items.Count(x => x.IndicatorCode.StartsWith("PI"));
        public int KRICount => this.Items.Count(x => x.IndicatorCode.StartsWith("KRI"));
        public int RICount => this.Items.Count(x => x.IndicatorCode.StartsWith("RI"));

        // Tiêu đề báo cáo
        public string ReportTitle
        {
            get
            {
                var departmentText = this.DepartmentId.HasValue ? " - Phòng ban" : "";
                return $"Báo cáo đo lường từ {this.FromDate:dd/MM/yyyy} đến {this.ToDate:dd/MM/yyyy}{departmentText}";
            }
        }
    }
} 