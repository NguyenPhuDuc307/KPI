namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    /// <summary>
    /// Dữ liệu cho widget hiển thị bảng KPI
    /// </summary>
    public class KpiTableWidgetData : WidgetData
    {
        /// <summary>
        /// Danh sách các chỉ số KPI
        /// </summary>
        public List<KpiItem> KpiItems { get; set; } = [];

        /// <summary>
        /// Khoảng thời gian
        /// </summary>
        public string TimePeriod { get; set; } = string.Empty;

        /// <summary>
        /// Có hiển thị phân trang không
        /// </summary>
        public bool ShowPagination { get; set; } = false;

        /// <summary>
        /// Số hàng trên một trang
        /// </summary>
        public int PageSize { get; set; } = 5;
    }

    /// <summary>
    /// Dữ liệu một chỉ số KPI hiển thị trong bảng
    /// </summary>
    public class KpiItem
    {
        /// <summary>
        /// Tên chỉ số
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mã chỉ số
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị hiện tại
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị mục tiêu
        /// </summary>
        public double? Target { get; set; }

        /// <summary>
        /// Xu hướng thay đổi (phần trăm +/-)
        /// </summary>
        public double? Trend { get; set; }

        /// <summary>
        /// Liên kết chi tiết
        /// </summary>
        public string DetailLink { get; set; } = string.Empty;
    }
}