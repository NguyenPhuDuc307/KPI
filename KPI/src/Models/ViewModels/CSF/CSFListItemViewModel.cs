using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.CSF
{
    /// <summary>
    /// View model for displaying Critical Success Factors in a list
    /// </summary>
    public class CSFListItemViewModel
    {
        /// <summary>
        /// CSF Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the CSF
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Code of the CSF
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Current Status of the CSF
        /// </summary>
        public CSFStatus Status { get; set; }

        /// <summary>
        /// Progress in percentage
        /// </summary>
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Success Factor ID this CSF belongs to
        /// </summary>
        public Guid SuccessFactorId { get; set; }

        /// <summary>
        /// Helper property to generate CSS class for status badges
        /// </summary>
        public string StatusBadgeClass
        {
            get
            {
                return Status switch
                {
                    CSFStatus.NotStarted => "badge bg-secondary",
                    CSFStatus.InProgress => "badge bg-primary",
                    CSFStatus.Delayed => "badge bg-warning text-dark",
                    CSFStatus.Completed => "badge bg-success",
                    CSFStatus.Cancelled => "badge bg-danger",
                    CSFStatus.AtRisk => "badge bg-danger",
                    _ => "badge bg-secondary"
                };
            }
        }

        /// <summary>
        /// Status display name
        /// </summary>
        public string StatusDisplay
        {
            get
            {
                return Status switch
                {
                    CSFStatus.NotStarted => "Chưa bắt đầu",
                    CSFStatus.InProgress => "Đang tiến hành",
                    CSFStatus.Delayed => "Bị trì hoãn",
                    CSFStatus.Completed => "Hoàn thành",
                    CSFStatus.Cancelled => "Đã hủy",
                    CSFStatus.AtRisk => "Có rủi ro",
                    _ => Status.ToString()
                };
            }
        }
    }
}