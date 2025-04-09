namespace KPISolution.Models.Enums.Visualization
{
    /// <summary>
    /// Các loại biểu đồ cho trực quan hóa dữ liệu
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// Biểu đồ cột
        /// </summary>
        [Display(Name = "Biểu đồ cột")]
        Bar = 1,

        /// <summary>
        /// Biểu đồ đường
        /// </summary>
        [Display(Name = "Biểu đồ đường")]
        Line = 2,

        /// <summary>
        /// Biểu đồ tròn
        /// </summary>
        [Display(Name = "Biểu đồ tròn")]
        Pie = 3,

        /// <summary>
        /// Biểu đồ bánh vòng
        /// </summary>
        [Display(Name = "Biểu đồ bánh vòng")]
        Doughnut = 4,

        /// <summary>
        /// Biểu đồ vùng
        /// </summary>
        [Display(Name = "Biểu đồ vùng")]
        Area = 5,

        /// <summary>
        /// Biểu đồ radar
        /// </summary>
        [Display(Name = "Biểu đồ radar")]
        Radar = 6,

        /// <summary>
        /// Biểu đồ nhiệt
        /// </summary>
        [Display(Name = "Biểu đồ nhiệt")]
        Heatmap = 7,

        /// <summary>
        /// Biểu đồ phân tán
        /// </summary>
        [Display(Name = "Biểu đồ phân tán")]
        Scatter = 8,

        /// <summary>
        /// Bảng dữ liệu
        /// </summary>
        [Display(Name = "Bảng")]
        Table = 9,

        /// <summary>
        /// Đồng hồ đo (Gauge)
        /// </summary>
        [Display(Name = "Đồng hồ đo")]
        Gauge = 10,

        /// <summary>
        /// Thẻ thông tin (Card)
        /// </summary>
        [Display(Name = "Thẻ thông tin")]
        Card = 11
    }

    /// <summary>
    /// Loại mục trên dashboard
    /// </summary>
    public enum DashboardItemType
    {
        /// <summary>
        /// Biểu đồ
        /// </summary>
        [Display(Name = "Biểu đồ")]
        Chart = 1,

        /// <summary>
        /// Thẻ chỉ số
        /// </summary>
        [Display(Name = "Thẻ chỉ số")]
        IndicatorCard = 2,

        /// <summary>
        /// Bảng dữ liệu
        /// </summary>
        [Display(Name = "Bảng")]
        Table = 3,

        /// <summary>
        /// Thanh tiến độ
        /// </summary>
        [Display(Name = "Thanh tiến độ")]
        ProgressBar = 4,

        /// <summary>
        /// Chỉ số hoặc số liệu
        /// </summary>
        [Display(Name = "Chỉ số")]
        Metric = 5,

        /// <summary>
        /// Widget tùy chỉnh
        /// </summary>
        [Display(Name = "Widget tùy chỉnh")]
        CustomWidget = 6,

        /// <summary>
        /// Văn bản
        /// </summary>
        [Display(Name = "Văn bản")]
        Text = 7,

        /// <summary>
        /// Hình ảnh
        /// </summary>
        [Display(Name = "Hình ảnh")]
        Image = 8
    }

    /// <summary>
    /// Khoảng thời gian cho lọc và so sánh dữ liệu trên dashboard
    /// </summary>
    public enum DisplayTimePeriod
    {
        /// <summary>
        /// Khoảng thời gian tùy chỉnh
        /// </summary>
        [Display(Name = "Tùy chỉnh")]
        Custom = 0,

        /// <summary>
        /// Hôm nay
        /// </summary>
        [Display(Name = "Hôm nay")]
        Today = 1,

        /// <summary>
        /// Hôm qua
        /// </summary>
        [Display(Name = "Hôm qua")]
        Yesterday = 2,

        /// <summary>
        /// Tuần này
        /// </summary>
        [Display(Name = "Tuần này")]
        ThisWeek = 3,

        /// <summary>
        /// Tuần trước
        /// </summary>
        [Display(Name = "Tuần trước")]
        LastWeek = 4,

        /// <summary>
        /// Tháng này
        /// </summary>
        [Display(Name = "Tháng này")]
        ThisMonth = 5,

        /// <summary>
        /// Tháng trước
        /// </summary>
        [Display(Name = "Tháng trước")]
        LastMonth = 6,

        /// <summary>
        /// 30 ngày qua
        /// </summary>
        [Display(Name = "30 ngày qua")]
        Last30Days = 7,

        /// <summary>
        /// Quý này
        /// </summary>
        [Display(Name = "Quý này")]
        ThisQuarter = 8,

        /// <summary>
        /// Quý trước
        /// </summary>
        [Display(Name = "Quý trước")]
        LastQuarter = 9,

        /// <summary>
        /// Năm nay
        /// </summary>
        [Display(Name = "Năm nay")]
        ThisYear = 10,

        /// <summary>
        /// Năm trước
        /// </summary>
        [Display(Name = "Năm trước")]
        LastYear = 11,

        /// <summary>
        /// Từ đầu năm đến nay
        /// </summary>
        [Display(Name = "Từ đầu năm đến nay")]
        YearToDate = 12
    }
}