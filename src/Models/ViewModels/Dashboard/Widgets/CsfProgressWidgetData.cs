namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    /// <summary>
    /// Dữ liệu cho widget hiển thị tiến độ các yếu tố thành công quan trọng (CSF)
    /// </summary>
    public class CsfProgressWidgetData : WidgetData
    {
        /// <summary>
        /// Danh sách các CSF
        /// </summary>
        public List<CsfItem> CsfItems { get; set; } = new List<CsfItem>();

        /// <summary>
        /// Có hiển thị tiêu đề không
        /// </summary>
        public bool ShowHeader { get; set; } = true;

        /// <summary>
        /// Có hiển thị chi tiết không
        /// </summary>
        public bool ShowDetails { get; set; } = true;

        /// <summary>
        /// Có hiển thị nút hành động không
        /// </summary>
        public bool ShowActions { get; set; } = true;
    }

    /// <summary>
    /// Dữ liệu một CSF hiển thị trong widget
    /// </summary>
    public class CsfItem
    {
        /// <summary>
        /// Định danh
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Mã CSF
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Tên CSF
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Phần trăm hoàn thành
        /// </summary>
        public decimal CompletionPercentage { get; set; }

        /// <summary>
        /// Trạng thái CSF
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Liên kết chi tiết
        /// </summary>
        public string DetailLink { get; set; } = string.Empty;
    }
}