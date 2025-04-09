namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    /// <summary>
    /// Dữ liệu cho widget hiển thị văn bản
    /// </summary>
    public class TextWidgetData : WidgetData
    {
        /// <summary>
        /// Nội dung văn bản
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Có xử lý markdown không
        /// </summary>
        public bool ParseMarkdown { get; set; } = false;

        /// <summary>
        /// Nội dung đã được xử lý từ markdown
        /// </summary>
        public string ParsedContent { get; set; } = string.Empty;

        /// <summary>
        /// Có phải là HTML không
        /// </summary>
        public bool IsHtml { get; set; } = false;
    }
}
