namespace KPISolution.Models.ViewModels.SuccessFactor
{
    public class SuccessFactorEditViewModel
    {
        public Guid? Id { get; init; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Name")]
        public string Name { get; init; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string Description { get; init; } = string.Empty;

        [Required(ErrorMessage = "Code is required")]
        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        [Display(Name = "Code")]
        public string Code { get; init; } = string.Empty;

        [Display(Name = "Critical Success Factor")]
        public bool IsCritical { get; init; }

        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; init; }

        [Display(Name = "Weight")]
        [Range(0, 100, ErrorMessage = "Weight must be between 0 and 100")]
        public decimal? Weight { get; init; }

        [Display(Name = "Status")]
        public SuccessFactorStatus Status { get; init; }

        [Display(Name = "Parent Success Factor")]
        public Guid? ParentId { get; init; }

        [Required(ErrorMessage = "Objective is required")]
        [Display(Name = "Objective")]
        public Guid ObjectiveId { get; init; }

        [Required(ErrorMessage = "Phòng ban là bắt buộc")]
        [Display(Name = "Phòng ban")]
        public Guid DepartmentId { get; init; }

        [Display(Name = "Progress")]
        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100")]
        public int ProgressPercentage { get; init; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; init; }

        [Display(Name = "Target Date")]
        [DataType(DataType.Date)]
        public DateTime? TargetDate { get; init; }
    }
}
