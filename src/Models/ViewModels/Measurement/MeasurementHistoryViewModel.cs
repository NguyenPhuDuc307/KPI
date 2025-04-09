using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Models.ViewModels.Measurement
{
    /// <summary>
    /// ViewModel hiển thị lịch sử đo lường của một chỉ số, bao gồm cả phân trang và bộ lọc
    /// </summary>
    public class MeasurementHistoryViewModel
    {
        /// <summary>
        /// ID của chỉ số
        /// </summary>
        public Guid IndicatorId { get; set; }

        /// <summary>
        /// Loại chỉ số
        /// </summary>
        public IndicatorType IndicatorType { get; set; }

        /// <summary>
        /// Tên chỉ số
        /// </summary>
        [Display(Name = "Tên chỉ số")]
        public string IndicatorName { get; set; } = string.Empty;

        /// <summary>
        /// Mã chỉ số
        /// </summary>
        [Display(Name = "Mã chỉ số")]
        public string IndicatorCode { get; set; } = string.Empty;

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        [Display(Name = "Đơn vị")]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị mục tiêu
        /// </summary>
        [Display(Name = "Mục tiêu")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Phòng ban
        /// </summary>
        [Display(Name = "Phòng ban")]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách các phép đo cho trang hiện tại
        /// </summary>
        public List<MeasurementItemViewModel> Measurements { get; set; } = [];

        /// <summary>
        /// Bộ lọc
        /// </summary>
        [Display(Name = "Chỉ số")]
        public Guid? SelectedIndicatorId { get; set; }
        public List<SelectListItem> IndicatorList { get; set; } = [];

        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Phân trang
        /// </summary>
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(this.TotalCount / (double)this.PageSize);
        public bool HasPreviousPage => this.CurrentPage > 1;
        public bool HasNextPage => this.CurrentPage < this.TotalPages;

        /// <summary>
        /// Số lượng phép đo
        /// </summary>
        [Display(Name = "Số phép đo")]
        public int MeasurementCount => this.Measurements?.Count ?? 0;

        /// <summary>
        /// Giá trị đo mới nhất
        /// </summary>
        [Display(Name = "Giá trị mới nhất")]
        public decimal? LatestValue => this.Measurements?.FirstOrDefault()?.Value;

        /// <summary>
        /// Ngày đo mới nhất
        /// </summary>
        [Display(Name = "Cập nhật gần nhất")]
        public DateTime? LatestDate => this.Measurements?.FirstOrDefault()?.MeasurementDate;

        /// <summary>
        /// Constructor
        /// </summary>
        public MeasurementHistoryViewModel()
        {
            this.IndicatorList = [];
        }
    }

    /// <summary>
    /// ViewModel cho một mục trong lịch sử đo lường
    /// </summary>
    public class MeasurementItemViewModel
    {
        /// <summary>
        /// ID của phép đo
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Giá trị đo được
        /// </summary>
        [Display(Name = "Giá trị")]
        public decimal Value { get; set; }

        /// <summary>
        /// Ngày thực hiện đo lường
        /// </summary>
        [Display(Name = "Ngày đo")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// Kỳ đo lường
        /// </summary>
        [Display(Name = "Kỳ")]
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// Phần trăm đạt được so với mục tiêu
        /// </summary>
        [Display(Name = "% đạt được")]
        [DisplayFormat(DataFormatString = "{0:0.##}%")]
        public decimal AchievementPercentage { get; set; }

        /// <summary>
        /// Chênh lệch so với mục tiêu
        /// </summary>
        [Display(Name = "Chênh lệch")]
        public decimal Variance { get; set; }

        /// <summary>
        /// Trạng thái phép đo
        /// </summary>
        [Display(Name = "Trạng thái")]
        public MeasurementStatus Status { get; set; }

        /// <summary>
        /// Ghi chú về phép đo
        /// </summary>
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; }
    }
}
