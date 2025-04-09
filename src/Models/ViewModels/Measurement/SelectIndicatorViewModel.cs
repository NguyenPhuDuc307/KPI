namespace KPISolution.Models.ViewModels.Measurement
{
    /// <summary>
    /// ViewModel để hiển thị trang chọn chỉ số trước khi tạo đo lường
    /// </summary>
    public class SelectIndicatorViewModel
    {
        /// <summary>
        /// Danh sách các chỉ số hiệu suất
        /// </summary>
        public List<IndicatorSelectionItemViewModel> PerformanceIndicators { get; set; } = [];

        /// <summary>
        /// Danh sách các chỉ số kết quả
        /// </summary>
        public List<IndicatorSelectionItemViewModel> ResultIndicators { get; set; } = [];
    }

    /// <summary>
    /// ViewModel cho mỗi item chỉ số hiển thị trong danh sách chọn
    /// </summary>
    public class IndicatorSelectionItemViewModel
    {
        /// <summary>
        /// ID của chỉ số
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Mã chỉ số
        /// </summary>
        [Display(Name = "Mã chỉ số")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Tên chỉ số
        /// </summary>
        [Display(Name = "Tên chỉ số")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Phòng ban phụ trách
        /// </summary>
        [Display(Name = "Phòng ban")]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Có phải là chỉ số quan trọng
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// Loại chỉ số
        /// </summary>
        public IndicatorType Type { get; set; }

        /// <summary>
        /// Ngày đo lường gần nhất
        /// </summary>
        [Display(Name = "Đo lường gần nhất")]
        public DateTime? LastMeasurementDate { get; set; }
    }
}