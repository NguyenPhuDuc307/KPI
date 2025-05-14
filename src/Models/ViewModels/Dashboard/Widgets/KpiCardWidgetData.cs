namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    /// <summary>
    /// Dữ liệu cho widget hiển thị thẻ KPI
    /// </summary>
    public class KpiCardWidgetData : WidgetData
    {
        /// <summary>
        /// Giá trị của KPI
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả
        /// </summary>
        public new string Description { get; set; } = string.Empty;

        /// <summary>
        /// Xu hướng thay đổi (phần trăm)
        /// </summary>
        public double Trend { get; set; }

        /// <summary>
        /// Giá trị mục tiêu
        /// </summary>
        public double? Target { get; set; }

        /// <summary>
        /// Khoảng thời gian
        /// </summary>
        public string TimePeriod { get; set; } = string.Empty;
    }
}