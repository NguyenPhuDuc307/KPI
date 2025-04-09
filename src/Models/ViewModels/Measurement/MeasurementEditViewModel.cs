using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.ViewModels.Measurement
{
    /// <summary>
    /// ViewModel cho form chỉnh sửa phép đo
    /// </summary>
    public class MeasurementEditViewModel
    {
        /// <summary>
        /// ID của phép đo
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID chung của Indicator
        /// </summary>
        public Guid IndicatorId { get; set; }

        /// <summary>
        /// ID của chỉ số loại Performance Indicator (nếu có)
        /// </summary>
        public Guid? PerformanceIndicatorId { get; set; }

        /// <summary>
        /// ID của chỉ số loại Result Indicator (nếu có)
        /// </summary>
        public Guid? ResultIndicatorId { get; set; }

        /// <summary>
        /// ID của chỉ số loại Success Factor (nếu có)
        /// </summary>
        public Guid? SuccessFactorId { get; set; }

        /// <summary>
        /// Tên chỉ số
        /// </summary>
        [Display(Name = "Chỉ số")]
        public string IndicatorName { get; set; } = string.Empty;

        /// <summary>
        /// Mã chỉ số
        /// </summary>
        [Display(Name = "Mã chỉ số")]
        public string IndicatorCode { get; set; } = string.Empty;

        /// <summary>
        /// Loại chỉ số
        /// </summary>
        [Display(Name = "Loại chỉ số")]
        public IndicatorType IndicatorType { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        [Display(Name = "Đơn vị")]
        public MeasurementUnit Unit { get; set; }

        /// <summary>
        /// Giá trị mục tiêu
        /// </summary>
        [Display(Name = "Giá trị mục tiêu")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Ngưỡng cảnh báo tối thiểu
        /// </summary>
        [Display(Name = "Ngưỡng cảnh báo tối thiểu")]
        public decimal? MinAlertThreshold { get; set; }

        /// <summary>
        /// Ngưỡng cảnh báo tối đa
        /// </summary>
        [Display(Name = "Ngưỡng cảnh báo tối đa")]
        public decimal? MaxAlertThreshold { get; set; }

        /// <summary>
        /// Ngày thực hiện đo lường
        /// </summary>
        [Required(ErrorMessage = "Ngày đo lường không được để trống")]
        [Display(Name = "Ngày đo lường")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// Giá trị đo được
        /// </summary>
        [Required(ErrorMessage = "Giá trị không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá trị đo được")]
        public decimal Value { get; set; }

        /// <summary>
        /// Trạng thái phép đo
        /// </summary>
        [Display(Name = "Trạng thái")]
        public MeasurementStatus Status { get; set; }

        /// <summary>
        /// Danh sách các trạng thái cho dropdown
        /// </summary>
        public IEnumerable<SelectListItem> StatusList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Ghi chú về phép đo
        /// </summary>
        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }

        // UI helper properties
        public string PageTitle => "Cập nhật phép đo cho " + this.IndicatorName;
        public string SubmitButtonText => "Cập nhật";
    }
}