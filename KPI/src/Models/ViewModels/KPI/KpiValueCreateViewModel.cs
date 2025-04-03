using System;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// ViewModel for creating a new KPI measurement
    /// </summary>
    public class KpiValueCreateViewModel
    {
        /// <summary>
        /// KPI ID
        /// </summary>
        public Guid KpiId { get; set; }

        /// <summary>
        /// KPI Code
        /// </summary>
        [Display(Name = "Mã KPI")]
        public string KpiCode { get; set; } = string.Empty;

        /// <summary>
        /// KPI Name
        /// </summary>
        [Display(Name = "Tên KPI")]
        public string KpiName { get; set; } = string.Empty;

        /// <summary>
        /// Target value
        /// </summary>
        [Display(Name = "Giá trị mục tiêu")]
        public double? TargetValue { get; set; }

        /// <summary>
        /// Actual value
        /// </summary>
        [Required(ErrorMessage = "Giá trị đo lường là bắt buộc")]
        [Display(Name = "Giá trị đo lường")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị phải lớn hơn hoặc bằng 0")]
        public double ActualValue { get; set; }

        /// <summary>
        /// Notes
        /// </summary>
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        /// <summary>
        /// Measurement date
        /// </summary>
        [Required(ErrorMessage = "Ngày đo lường là bắt buộc")]
        [Display(Name = "Ngày đo lường")]
        [DataType(DataType.Date)]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        [Display(Name = "Đơn vị")]
        public string? Unit { get; set; }

        /// <summary>
        /// Measurement frequency
        /// </summary>
        [Display(Name = "Tần suất đo")]
        public MeasurementFrequency Frequency { get; set; }
    }
}