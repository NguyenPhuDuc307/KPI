namespace KPISolution.Models.ViewModels.Objective
{
    public class ObjectiveDetailsViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Status")]
        public ObjectiveStatus Status { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; } = string.Empty;

        [Display(Name = "Department ID")]
        public Guid DepartmentId { get; set; }

        [Display(Name = "Progress")]
        public int ProgressPercentage { get; set; }

        // Success factors linked to this objective
        public IEnumerable<SuccessFactorListItemViewModel> SuccessFactors { get; set; } = new List<SuccessFactorListItemViewModel>();
    }
}
