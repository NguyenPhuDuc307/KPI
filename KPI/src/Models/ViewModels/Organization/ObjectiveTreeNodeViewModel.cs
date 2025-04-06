namespace KPISolution.Models.ViewModels.Organization
{
    /// <summary>
    /// View model representing a node in the objective tree structure
    /// </summary>
    public class ObjectiveTreeNodeViewModel
    {
        /// <summary>
        /// Unique identifier of the objective
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Unique code of the objective
        /// </summary>
        [Display(Name = "Mã mục tiêu")]
        public string Code { get; init; } = string.Empty;

        /// <summary>
        /// Name of the objective
        /// </summary>
        [Display(Name = "Tên Objective")]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Description of the objective
        /// </summary>
        [Display(Name = "Mô tả")]
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Business perspective this objective belongs to
        /// </summary>
        [Display(Name = "Phương diện kinh doanh")]
        public BusinessPerspective BusinessPerspective { get; init; }

        /// <summary>
        /// Priority level
        /// </summary>
        [Display(Name = "Độ ưu tiên")]
        public PriorityLevel Priority { get; init; }

        /// <summary>
        /// Current status
        /// </summary>
        [Display(Name = "Trạng thái")]
        public ObjectiveStatus Status { get; init; }

        /// <summary>
        /// Progress percentage (0-100)
        /// </summary>
        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; init; }

        /// <summary>
        /// Timeframe classification
        /// </summary>
        [Display(Name = "Khung thời gian")]
        public TimeframeType Timeframe { get; init; }

        /// <summary>
        /// Department ID responsible for this objective
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Department name
        /// </summary>
        [Display(Name = "Phòng ban")]
        public string? DepartmentName { get; init; }

        /// <summary>
        /// Child objectives in the tree structure
        /// </summary>
        public List<ObjectiveTreeNodeViewModel> Children { get; init; } = [];

        /// <summary>
        /// Whether this node has any children
        /// </summary>
        public bool HasChildren => this.Children != null && this.Children.Count > 0;

        /// <summary>
        /// Gets the appropriate CSS class for the progress bar based on progress percentage
        /// </summary>
        public string GetProgressBarClass()
        {
            if (this.ProgressPercentage >= 75) return "bg-success";
            if (this.ProgressPercentage >= 50) return "bg-info";
            if (this.ProgressPercentage >= 25) return "bg-warning";
            return "bg-danger";
        }

        /// <summary>
        /// Gets the appropriate CSS class for the status badge
        /// </summary>
        public string GetStatusBadgeClass()
        {
            return this.Status switch
            {
                ObjectiveStatus.Completed => "bg-success",
                ObjectiveStatus.InProgress => "bg-primary",
                ObjectiveStatus.Delayed => "bg-warning",
                ObjectiveStatus.AtRisk => "bg-danger",
                ObjectiveStatus.Cancelled => "bg-secondary",
                _ => "bg-light text-dark"
            };
        }

        /// <summary>
        /// Gets the appropriate icon for the priority level
        /// </summary>
        public string GetPriorityIcon()
        {
            return this.Priority switch
            {
                PriorityLevel.Critical => "bi-exclamation-triangle-fill text-danger",
                PriorityLevel.High => "bi-arrow-up-circle-fill text-warning",
                PriorityLevel.Medium => "bi-dash-circle-fill text-primary",
                PriorityLevel.Low => "bi-arrow-down-circle-fill text-info",
                _ => "bi-circle text-secondary"
            };
        }
    }
}
