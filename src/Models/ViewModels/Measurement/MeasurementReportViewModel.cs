using Microsoft.AspNetCore.Mvc.Rendering;

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
        public int TotalMeasurements => Items.Count;
        public int OnTargetCount => Items.Count(x => x.AchievementPercentage >= 100);
        public int BelowTargetCount => Items.Count(x => x.AchievementPercentage < 100);
        public decimal AverageAchievement => Items.Any() ? Items.Average(x => x.AchievementPercentage) : 0;

        // Phân loại theo loại chỉ số
        public int KPICount => Items.Count(x => x.IndicatorCode.StartsWith("KPI"));
        public int PICount => Items.Count(x => x.IndicatorCode.StartsWith("PI"));
        public int KRICount => Items.Count(x => x.IndicatorCode.StartsWith("KRI"));
        public int RICount => Items.Count(x => x.IndicatorCode.StartsWith("RI"));

        // Tiêu đề báo cáo
        public string ReportTitle
        {
            get
            {
                var departmentText = DepartmentId.HasValue ? " - Phòng ban" : "";
                return $"Báo cáo đo lường từ {FromDate:dd/MM/yyyy} đến {ToDate:dd/MM/yyyy}{departmentText}";
            }
        }
    }
} 