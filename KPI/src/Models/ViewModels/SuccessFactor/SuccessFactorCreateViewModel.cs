namespace KPISolution.Models.ViewModels.SuccessFactor
{
    public class SuccessFactorCreateViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Code is required")]
        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Critical Success Factor")]
        public bool IsCritical { get; set; }

        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        [Display(Name = "Weight")]
        [Range(0, 100, ErrorMessage = "Weight must be between 0 and 100")]
        public decimal? Weight { get; set; }

        [Display(Name = "Status")]
        public SuccessFactorStatus Status { get; set; } = SuccessFactorStatus.NotStarted;

        [Display(Name = "Parent Success Factor")]
        public Guid? ParentId { get; set; }

        [Required(ErrorMessage = "Objective is required")]
        [Display(Name = "Objective")]
        public Guid ObjectiveId { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; } = DateTime.Today;

        [Display(Name = "Target Date")]
        [DataType(DataType.Date)]
        public DateTime? TargetDate { get; set; } = DateTime.Today.AddMonths(3);

        [Required(ErrorMessage = "Phòng ban là bắt buộc")]
        [Display(Name = "Phòng ban")]
        public Guid DepartmentId { get; set; }
    }
}
