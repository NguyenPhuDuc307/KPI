using KPISolution.Models.Enums;
using KPISolution.Models.ViewModels.CSF;

namespace KPISolution.Models.ViewModels.SuccessFactor
{
    public class SuccessFactorDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public PriorityLevel Priority { get; set; }
        public ObjectiveStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public Guid BusinessObjectiveId { get; set; }
        public string? BusinessObjectiveName { get; set; }
        public string? Owner { get; set; }
        public string? Notes { get; set; }

        // Related CSFs
        public List<CSFListItemViewModel> CriticalSuccessFactors { get; set; } = new List<CSFListItemViewModel>();

        // Calculated properties
        public int DaysRemaining => (TargetDate - DateTime.Today).Days;
        public bool IsOnTrack => ProgressPercentage >= TimeElapsedPercentage;
        public int TimeElapsedPercentage
        {
            get
            {
                var totalDays = (TargetDate - StartDate).TotalDays;
                var elapsedDays = (DateTime.Now - StartDate).TotalDays;
                return totalDays > 0 ? (int)Math.Min(100, Math.Max(0, (elapsedDays / totalDays) * 100)) : 0;
            }
        }

        // Helper properties for UI display
        public string StatusBadgeClass
        {
            get
            {
                return Status switch
                {
                    ObjectiveStatus.NotStarted => "badge bg-secondary",
                    ObjectiveStatus.InProgress => "badge bg-primary",
                    ObjectiveStatus.OnHold => "badge bg-warning text-dark",
                    ObjectiveStatus.Completed => "badge bg-success",
                    ObjectiveStatus.Canceled => "badge bg-danger",
                    ObjectiveStatus.Delayed => "badge bg-info text-dark",
                    _ => "badge bg-secondary"
                };
            }
        }

        public string StatusDisplay
        {
            get
            {
                return Status switch
                {
                    ObjectiveStatus.NotStarted => "Chưa bắt đầu",
                    ObjectiveStatus.InProgress => "Đang tiến hành",
                    ObjectiveStatus.OnHold => "Tạm dừng",
                    ObjectiveStatus.Completed => "Hoàn thành",
                    ObjectiveStatus.Canceled => "Đã hủy",
                    ObjectiveStatus.Delayed => "Bị trì hoãn",
                    _ => Status.ToString()
                };
            }
        }

        public string PriorityBadgeClass
        {
            get
            {
                return Priority switch
                {
                    PriorityLevel.Low => "badge bg-success",
                    PriorityLevel.Medium => "badge bg-warning text-dark",
                    PriorityLevel.High => "badge bg-danger",
                    PriorityLevel.Critical => "badge bg-dark",
                    _ => "badge bg-secondary"
                };
            }
        }

        public string PriorityDisplay
        {
            get
            {
                return Priority switch
                {
                    PriorityLevel.Low => "Thấp",
                    PriorityLevel.Medium => "Trung bình",
                    PriorityLevel.High => "Cao",
                    PriorityLevel.Critical => "Nghiêm trọng",
                    _ => Priority.ToString()
                };
            }
        }

        public string ProgressBarClass
        {
            get
            {
                return ProgressPercentage switch
                {
                    100 => "progress-bar bg-success",
                    >= 75 => "progress-bar bg-info",
                    >= 50 => "progress-bar bg-primary",
                    >= 25 => "progress-bar bg-warning",
                    _ => "progress-bar bg-danger"
                };
            }
        }
    }
}