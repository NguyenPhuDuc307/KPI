using KPISolution.Models.Enums.Visualization;

namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    /// <summary>
    /// Dữ liệu cho widget hiển thị biểu đồ
    /// </summary>
    public class ChartWidgetData : WidgetData
    {
        /// <summary>
        /// Loại biểu đồ (Line, Bar, Pie, v.v.)
        /// </summary>
        public ChartType ChartType { get; set; } = ChartType.Line;

        /// <summary>
        /// Tiêu đề của biểu đồ
        /// </summary>
        public new string Title { get; set; } = string.Empty;

        /// <summary>
        /// Nhãn cho các điểm dữ liệu (trục X)
        /// </summary>
        public string[] Labels { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Các bộ dữ liệu của biểu đồ
        /// </summary>
        public ChartDataset[] Datasets { get; set; } = Array.Empty<ChartDataset>();

        /// <summary>
        /// Có hiển thị chú thích không
        /// </summary>
        public bool ShowLegend { get; set; } = true;

        /// <summary>
        /// Khoảng thời gian hiển thị (tuần, tháng, quý, năm)
        /// </summary>
        public DisplayTimePeriod TimePeriod { get; set; } = DisplayTimePeriod.ThisMonth;

        /// <summary>
        /// ID của widget
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Dữ liệu biểu đồ
        /// </summary>
        public object? ChartData { get; set; }

        /// <summary>
        /// Tùy chọn hiển thị biểu đồ
        /// </summary>
        public object? ChartOptions { get; set; }
    }

    /// <summary>
    /// Một bộ dữ liệu trong biểu đồ
    /// </summary>
    public class ChartDataset
    {
        /// <summary>
        /// Nhãn của bộ dữ liệu
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Dữ liệu số
        /// </summary>
        public double[] Data { get; set; } = Array.Empty<double>();

        /// <summary>
        /// Màu đường viền/cột/mục
        /// </summary>
        public string BorderColor { get; set; } = "#4e73df";

        /// <summary>
        /// Màu nền (cho area chart, bar chart)
        /// </summary>
        public string BackgroundColor { get; set; } = "rgba(78, 115, 223, 0.2)";

        /// <summary>
        /// Độ dày đường (cho line chart)
        /// </summary>
        public int BorderWidth { get; set; } = 2;

        /// <summary>
        /// Điểm dữ liệu có được làm tròn không
        /// </summary>
        public bool BorderRound { get; set; } = false;
    }
}