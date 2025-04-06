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
        public string Content { get; init; } = string.Empty;

        /// <summary>
        /// Có xử lý markdown không
        /// </summary>
        public bool ParseMarkdown { get; init; } = false;

        /// <summary>
        /// Nội dung đã được xử lý từ markdown
        /// </summary>
        public string ParsedContent { get; init; } = string.Empty;

        /// <summary>
        /// Có phải là HTML không
        /// </summary>
        public bool IsHtml { get; init; } = false;
    }
}
