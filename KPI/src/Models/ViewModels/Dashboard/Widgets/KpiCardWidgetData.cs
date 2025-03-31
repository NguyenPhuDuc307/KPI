using System;

namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    /// <summary>
    /// Dữ liệu cho widget hiển thị thông tin KPI theo dạng thẻ
    /// </summary>
    public class KpiCardWidgetData
    {
        /// <summary>
        /// ID của widget
        /// </summary>
        public Guid WidgetId { get; set; }

        /// <summary>
        /// ID của KPI
        /// </summary>
        public Guid KpiId { get; set; }

        /// <summary>
        /// Mã của KPI
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Tên của KPI
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị hiện tại của KPI
        /// </summary>
        public decimal CurrentValue { get; set; }

        /// <summary>
        /// Giá trị mục tiêu của KPI
        /// </summary>
        public decimal TargetValue { get; set; }

        /// <summary>
        /// Đơn vị đo lường
        /// </summary>
        public string MeasurementUnit { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái hiện tại của KPI
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Phần trăm tiến độ hiện tại
        /// </summary>
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Hướng xu hướng (tăng/giảm)
        /// </summary>
        public string TrendDirection { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị xu hướng
        /// </summary>
        public decimal TrendValue { get; set; }

        /// <summary>
        /// Ngày cập nhật cuối cùng
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}