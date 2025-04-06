namespace KPISolution.Models.ViewModels.Objective
{
    public class ObjectiveEditViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ObjectiveStatus Status { get; set; }

        [Required]
        [Display(Name = "Department")]
        public Guid DepartmentId { get; set; }

        [Range(0, 100)]
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; }
    }
}
