using System;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// ViewModel for displaying KPI information in the separated view
    /// </summary>
    public class KpiInfoViewModel
    {
        /// <summary>
        /// KPI ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// KPI Code
        /// </summary>
        [Display(Name = "Mã KPI")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// KPI Name
        /// </summary>
        [Display(Name = "Tên KPI")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// KPI Description
        /// </summary>
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        /// <summary>
        /// Department
        /// </summary>
        [Display(Name = "Phòng ban")]
        public string? Department { get; set; }

        /// <summary>
        /// Measurement frequency
        /// </summary>
        [Display(Name = "Tần suất đo")]
        public MeasurementFrequency Frequency { get; set; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        [Display(Name = "Đơn vị")]
        public string? Unit { get; set; }

        /// <summary>
        /// Target value
        /// </summary>
        [Display(Name = "Giá trị mục tiêu")]
        public double? TargetValue { get; set; }

        /// <summary>
        /// Actual value
        /// </summary>
        [Display(Name = "Giá trị hiện tại")]
        public double? ActualValue { get; set; }

        /// <summary>
        /// Percentage of target achieved
        /// </summary>
        [Display(Name = "% Hoàn thành")]
        public double AchievementPercentage
        {
            get
            {
                if (TargetValue.HasValue && TargetValue > 0 && ActualValue.HasValue)
                {
                    return Math.Round((ActualValue.Value / TargetValue.Value) * 100, 2);
                }
                return 0;
            }
        }

        /// <summary>
        /// Type of KPI (KPI, PI, KRI, RI)
        /// </summary>
        [Display(Name = "Loại KPI")]
        public string? Type { get; set; }

        /// <summary>
        /// CSS class for the status badge
        /// </summary>
        public string StatusCssClass { get; set; } = "badge bg-secondary";

        /// <summary>
        /// Status text
        /// </summary>
        [Display(Name = "Trạng thái")]
        public string Status
        {
            get
            {
                if (AchievementPercentage >= 100)
                {
                    return "Đạt mục tiêu";
                }
                else if (AchievementPercentage >= 80)
                {
                    return "Có rủi ro";
                }
                else
                {
                    return "Không đạt";
                }
            }
        }
    }
}